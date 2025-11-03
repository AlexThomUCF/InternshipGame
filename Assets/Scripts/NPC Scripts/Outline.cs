using UnityEngine;

public class Outline : MonoBehaviour
{
    private Renderer[] renderers;
    private Material[] originalMaterials;
    public Material outlineMaterial;

    private bool outlined = false;

    void Awake()
    {
        // Collect all renderers from this object and its children
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterial;
        }
    }

    public void EnableOutline()
    {
        if (outlined || outlineMaterial == null) return;
        outlined = true;

        foreach (var r in renderers)
        {
            // Add the outline material as an extra layer
            var mats = r.sharedMaterials;
            var newMats = new Material[mats.Length + 1];
            mats.CopyTo(newMats, 0);
            newMats[mats.Length] = outlineMaterial;
            r.sharedMaterials = newMats;
        }
    }

    public void DisableOutline()
    {
        if (!outlined) return;
        outlined = false;

        foreach (var r in renderers)
        {
            // Remove the outline material if present
            var mats = r.sharedMaterials;
            if (mats.Length > 1)
            {
                var newMats = new Material[mats.Length - 1];
                for (int i = 0; i < newMats.Length; i++)
                    newMats[i] = mats[i];
                r.sharedMaterials = newMats;
            }
        }
    }
}
