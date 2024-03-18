using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class FloatingUI : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float floatRange = 10f;
    [SerializeField] float floatSpeed = 1f;
    private Vector2 startPosition;
    private Vector2 targetPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
        CalculateTargetPosition();
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time*floatSpeed, 1f);
        rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

        if (t == 1f)
        {
            CalculateTargetPosition();
        }
    }

    private void CalculateTargetPosition()
    {
        float offsetX = Random.Range(-floatRange, floatRange);
        float offsetY = Random.Range(-floatRange, floatRange);
        targetPosition = startPosition + new Vector2(offsetX, offsetY);
    }
}
