using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighLightText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color defaultColor;
    [SerializeField] Color highlightColor;

    public void Start()
    {
        defaultColor = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = defaultColor;
    }
}
