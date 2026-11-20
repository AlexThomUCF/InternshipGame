
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text buttonText;
    public Camera mainCamera;

    [Header("Button Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    [Header("Background Colors")]
    public Color defaultBackGroundColor;
    public Color backGroundHoverColor;

    [Header("Screen Flash")]
    public Image flashImage;
    public float flashDuration = 0.15f;

    void Start()
    {
        buttonText.color = normalColor;

        if (flashImage != null)
            flashImage.color = new Color(1, 1, 1, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        mainCamera.backgroundColor = backGroundHoverColor;

        if (flashImage != null)
            StartCoroutine(FlashEffect());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
        mainCamera.backgroundColor = defaultBackGroundColor;
    }

    IEnumerator FlashEffect()
    {
        float timer = 0;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / flashDuration);

            flashImage.color = new Color(1, 1, 1, alpha);

            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0);
    }
}
