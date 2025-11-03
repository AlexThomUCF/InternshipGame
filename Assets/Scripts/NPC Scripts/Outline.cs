using UnityEngine;
using System.Collections.Generic;

public class Outline : MonoBehaviour
{
    [Tooltip("Material used for outlining the character.")]
    public Material outlineMaterial;

    private List<Renderer> renderers = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMats = new Dictionary<Renderer, Material[]>();
    private bool outlined;

    void Awake()
    {
        // Collect ALL renderers in this object and any depth of children (even nested)
        var allRenderers = GetComponentsInChildren<Renderer>(true);
        renderers.AddRange(allRenderers);

        foreach (var r in renderers)
        {
            if (!originalMats.ContainsKey(r))
                originalMats[r] = r.sharedMaterials;
        }
    }

    public void EnableOutline(bool enable)
    {
        if (enable && !outlined)
        {
            foreach (var r in renderers)
            {
                if (r == null) continue;

                var mats = new Material[originalMats[r].Length + 1];
                originalMats[r].CopyTo(mats, 0);
                mats[mats.Length - 1] = outlineMaterial;
                r.materials = mats;
            }
            outlined = true;
        }
        else if (!enable && outlined)
        {
            foreach (var r in renderers)
            {
                if (r == null) continue;

                if (originalMats.TryGetValue(r, out var mats))
                    r.materials = mats;
            }
            outlined = false;
        }
    }
}
