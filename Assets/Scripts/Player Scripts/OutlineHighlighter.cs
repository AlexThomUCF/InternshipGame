using UnityEngine;

public class OutlineHighlighter : MonoBehaviour
{
    [Header("Aim Settings")]
    public Camera mainCamera;
    public float maxDistance = 100f;

    private Outline currentOutline;

    void Update()
    {
        HandleAim();
    }

    void HandleAim()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            Outline outline = hit.collider.GetComponentInParent<Outline>();

            if (outline != null)
            {
                if (currentOutline != outline)
                {
                    ClearCurrentOutline();
                    currentOutline = outline;
                    currentOutline.EnableOutline();
                }
                return;
            }
        }

        ClearCurrentOutline();
    }

    void ClearCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.DisableOutline();
            currentOutline = null;
        }
    }
}

