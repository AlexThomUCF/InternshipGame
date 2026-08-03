using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmisionController : MonoBehaviour
{
    public Renderer targetRenderer;
    public Color emissionColor = Color.cyan;
    public float normalIntensity = 1f;
    public float flashIntensity = 5f;

    private Material mat;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        mat = targetRenderer.material;
        mat.EnableKeyword("_EMISSION");

        SetEmissionIntensity(normalIntensity);
    }

    public void SetHighEmission()
    {
        SetEmissionIntensity(flashIntensity);
    }

    public void SetNormalEmission()
    {
        SetEmissionIntensity(normalIntensity);
    }

    private void SetEmissionIntensity(float intensity)
    {
        mat.SetColor("_EmissionColor", emissionColor * intensity);
    }
}
