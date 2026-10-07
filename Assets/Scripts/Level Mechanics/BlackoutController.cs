using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BlackoutController : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private List<Light> lights = new();

    [Header("Blackout UI")]
    [SerializeField] private Image blackoutImage;
    [SerializeField] private float fadeSpeed = 2f;

    [Header("Timing")]
    [SerializeField] private float minBlackoutInterval = 60f;
    [SerializeField] private float maxBlackoutInterval = 120f;
    [SerializeField] private float flickerDuration = 2f;
    [SerializeField] private float blackoutDuration = 3f;

    [Header("Light Flicker")]
    [SerializeField] private float flickerMinIntensity = 0.1f;
    [SerializeField] private float flickerSpeed = 0.1f;

    [Header("Canvas Flicker")]
    [SerializeField] private float canvasFlickerMinAlpha = 0.05f;
    [SerializeField] private float canvasFlickerMaxAlpha = 0.15f;
    [SerializeField] private float canvasFlickerSpeed = 0.05f;

    [Header("NPC Shuffle")]
    [SerializeField] private List<Transform> npcWaypoints = new();
    [SerializeField] private LayerMask characterLayer;

    private Dictionary<Light, float> originalIntensities =
        new Dictionary<Light, float>();

    private void Start()
    {
        CacheLightData();

        StartCoroutine(
            BlackoutLoop()
        );
    }

    private void CacheLightData()
    {
        originalIntensities.Clear();

        foreach (Light light in lights)
        {
            if (light != null &&
                !originalIntensities.ContainsKey(light))
            {
                originalIntensities.Add(
                    light,
                    light.intensity
                );
            }
        }

        if (blackoutImage != null)
        {
            blackoutImage.color =
                new Color(0, 0, 0, 0);
        }
    }

    private IEnumerator BlackoutLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(
                    minBlackoutInterval,
                    maxBlackoutInterval
                )
            );

            yield return StartCoroutine(
                FlickerLights()
            );

            yield return StartCoroutine(
                DoBlackout()
            );
        }
    }

    private IEnumerator FlickerLights()
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            foreach (Light light in lights)
            {
                if (light == null)
                    continue;

                light.intensity =
                    Random.Range(
                        flickerMinIntensity,
                        originalIntensities[light]
                    );
            }

            if (blackoutImage != null)
            {
                float alpha =
                    Random.Range(
                        canvasFlickerMinAlpha,
                        canvasFlickerMaxAlpha
                    );

                blackoutImage.color =
                    new Color(0, 0, 0, alpha);
            }

            timer += flickerSpeed;

            yield return new WaitForSeconds(
                flickerSpeed
            );
        }

        if (blackoutImage != null)
        {
            blackoutImage.color =
                new Color(0, 0, 0, 0);
        }
    }

    private IEnumerator DoBlackout()
    {
        // Turn off lights.
        foreach (Light light in lights)
        {
            if (light != null)
                light.enabled = false;
        }

        // Fade completely black.
        yield return StartCoroutine(
            FadeCanvas(0f, 1f)
        );

        // Stop NPCs before moving them.
        CancelNPCActions();

        // Move NPCs while the screen is completely black.
        RepositionNPCs();

        // Stay black.
        yield return new WaitForSeconds(
            blackoutDuration
        );

        // Fade back in.
        yield return StartCoroutine(
            FadeCanvas(1f, 0f)
        );

        // Turn lights back on.
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.enabled = true;

                light.intensity =
                    originalIntensities[light];
            }
        }
    }

    private IEnumerator FadeCanvas(
        float from,
        float to
    )
    {
        float t = 0f;

        while (t < 1f)
        {
            t +=
                Time.deltaTime *
                fadeSpeed;

            float alpha =
                Mathf.Lerp(
                    from,
                    to,
                    t
                );

            if (blackoutImage != null)
            {
                blackoutImage.color =
                    new Color(
                        0,
                        0,
                        0,
                        alpha
                    );
            }

            yield return null;
        }
    }

    private void CancelNPCActions()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                999f,
                characterLayer
            );

        List<Transform> npcs =
            new List<Transform>();

        foreach (Collider hit in hits)
        {
            Transform root =
                hit.transform.root;

            if (!npcs.Contains(root))
            {
                npcs.Add(root);
            }
        }

        foreach (Transform npc in npcs)
        {
            AINavigation navigation =
                npc.GetComponent<AINavigation>();

            if (navigation != null)
            {
                navigation.CancelCurrentAction();
            }
        }
    }

    private void RepositionNPCs()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                999f,
                characterLayer
            );

        List<Transform> npcs =
            new List<Transform>();

        foreach (Collider hit in hits)
        {
            Transform root =
                hit.transform.root;

            if (!npcs.Contains(root))
            {
                npcs.Add(root);
            }
        }

        if (npcs.Count == 0)
        {
            Debug.LogWarning(
                "BlackoutController: No NPCs found."
            );

            return;
        }

        if (npcWaypoints.Count < npcs.Count)
        {
            Debug.LogWarning(
                "BlackoutController: Not enough NPC waypoints."
            );

            return;
        }

        List<Transform> availableWaypoints =
            new List<Transform>(
                npcWaypoints
            );

        foreach (Transform npc in npcs)
        {
            int index =
                Random.Range(
                    0,
                    availableWaypoints.Count
                );

            Transform waypoint =
                availableWaypoints[index];

            NavMeshAgent agent =
                npc.GetComponent<NavMeshAgent>();

            if (agent != null &&
                agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();

                // Make sure the waypoint is actually
                // on the NavMesh.
                NavMeshHit navHit;

                if (NavMesh.SamplePosition(
                    waypoint.position,
                    out navHit,
                    3f,
                    NavMesh.AllAreas))
                {
                    bool warped =
                        agent.Warp(
                            navHit.position
                        );

                    if (!warped)
                    {
                        Debug.LogWarning(
                            npc.name +
                            " failed to warp during blackout."
                        );
                    }
                }
                else
                {
                    Debug.LogWarning(
                        "Could not find NavMesh near waypoint " +
                        waypoint.name
                    );
                }

                agent.ResetPath();
                agent.isStopped = true;
            }
            else
            {
                // Fallback if the agent isn't currently
                // connected to the NavMesh.
                NavMeshHit navHit;

                if (NavMesh.SamplePosition(
                    waypoint.position,
                    out navHit,
                    3f,
                    NavMesh.AllAreas))
                {
                    npc.position =
                        navHit.position;
                }
                else
                {
                    npc.position =
                        waypoint.position;
                }
            }

            npc.rotation =
                waypoint.rotation;

            AINavigation navigation =
                npc.GetComponent<AINavigation>();

            if (navigation != null)
            {
                navigation.ResetAfterBlackout();
            }

            availableWaypoints.RemoveAt(
                index
            );
        }
    }
}