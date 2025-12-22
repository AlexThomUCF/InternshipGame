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

    private Dictionary<Light, float> originalIntensities = new();

    private void Start()
    {
        CacheLightData();
        StartCoroutine(BlackoutLoop());
    }

    private void CacheLightData()
    {
        originalIntensities.Clear();

        foreach (Light light in lights)
        {
            if (light != null && !originalIntensities.ContainsKey(light))
                originalIntensities.Add(light, light.intensity);
        }

        if (blackoutImage != null)
            blackoutImage.color = new Color(0, 0, 0, 0);
    }

    private IEnumerator BlackoutLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minBlackoutInterval, maxBlackoutInterval));

            yield return StartCoroutine(FlickerLights());
            yield return StartCoroutine(DoBlackout());
        }
    }

    private IEnumerator FlickerLights()
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            foreach (Light light in lights)
            {
                if (light == null) continue;

                light.intensity = Random.Range(
                    flickerMinIntensity,
                    originalIntensities[light]
                );
            }

            // Canvas micro-flicker
            if (blackoutImage != null)
            {
                float alpha = Random.Range(canvasFlickerMinAlpha, canvasFlickerMaxAlpha);
                blackoutImage.color = new Color(0, 0, 0, alpha);
            }

            timer += flickerSpeed;
            yield return new WaitForSeconds(flickerSpeed);
        }

        if (blackoutImage != null)
            blackoutImage.color = new Color(0, 0, 0, 0);
    }

    private IEnumerator DoBlackout()
    {
        // Turn off all lights
        foreach (Light light in lights)
            if (light != null)
                light.enabled = false;

        // Fade canvas to full black first
        yield return StartCoroutine(FadeCanvas(0f, 1f));

        // Now reposition all NPCs while the screen is fully black
        RepositionNPCs();

        // Hold blackout
        yield return new WaitForSeconds(blackoutDuration);

        // Fade canvas back
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        // Restore lights
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.enabled = true;
                light.intensity = originalIntensities[light];
            }
        }
    }

    private IEnumerator FadeCanvas(float from, float to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            float alpha = Mathf.Lerp(from, to, t);

            if (blackoutImage != null)
                blackoutImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }
    }

    private void RepositionNPCs()
    {
        // Get all colliders on the Characters layer
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            999f,
            characterLayer
        );

        List<Transform> npcs = new List<Transform>();

        foreach (Collider hit in hits)
        {
            Transform root = hit.transform.root;
            if (!npcs.Contains(root))
                npcs.Add(root);
        }

        if (npcs.Count == 0 || npcWaypoints.Count < npcs.Count)
        {
            Debug.LogWarning("BlackoutController: Not enough NPCs or waypoints.");
            return;
        }

        // Assign unique waypoints
        List<Transform> availableWaypoints = new List<Transform>(npcWaypoints);

        foreach (Transform npc in npcs)
        {
            int index = Random.Range(0, availableWaypoints.Count);
            Transform waypoint = availableWaypoints[index];

            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
                agent.Warp(waypoint.position);
            else
                npc.position = waypoint.position;

            npc.rotation = waypoint.rotation;
            availableWaypoints.RemoveAt(index);
        }
    }
}