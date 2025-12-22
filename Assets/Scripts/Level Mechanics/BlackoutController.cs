using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Flicker")]
    [SerializeField] private float flickerMinIntensity = 0.1f;
    [SerializeField] private float flickerSpeed = 0.1f;

    [Header("Canvas Flicker")]
    [SerializeField] private float canvasFlickerMinAlpha = 0.05f;
    [SerializeField] private float canvasFlickerMaxAlpha = 0.2f;
    [SerializeField] private float canvasFlickerSpeed = 0.05f;

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

        timer += canvasFlickerSpeed;
        yield return new WaitForSeconds(canvasFlickerSpeed);
    }

    // Ensure canvas is transparent before full blackout
    if (blackoutImage != null)
        blackoutImage.color = new Color(0, 0, 0, 0);
}

        private IEnumerator DoBlackout()
        {
            // Turn lights off
         foreach (Light light in lights)
                if (light != null)
                    light.enabled = false;

            // Fade to black
            yield return StartCoroutine(FadeCanvas(0f, 1f));

            yield return new WaitForSeconds(blackoutDuration);

            // Fade back in
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
}
