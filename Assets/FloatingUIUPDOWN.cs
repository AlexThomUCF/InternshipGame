using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUIUPDOWN : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatAmount = 10f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * floatSpeed) * floatAmount;

        rectTransform.anchoredPosition = startPosition + new Vector2(0, y);
    }
}
