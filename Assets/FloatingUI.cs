
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    private float floatSpeed;
    private float floatAmount;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;

        floatSpeed = Random.Range(2f, 4f);
        floatAmount = Random.Range(10f, 12f);
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        float y = Mathf.Cos(Time.time * floatSpeed) * floatAmount;

        rectTransform.anchoredPosition = startPosition + new Vector2(x, y);
    }
}
