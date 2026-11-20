
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text buttonText;
    public Camera mainCamera;

    [Header("Button Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    [Header("Button Colors")]
    public Color defaultBackGroundColor;
    public Color backGroundHoverColor;

    void Start()
    {
        buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        mainCamera.backgroundColor = backGroundHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
        mainCamera.backgroundColor = defaultBackGroundColor;
    }
}
