using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item")]
    [SerializeField] Rarity rarity;
    [SerializeField] string itemName;
    [SerializeField] string flavorText;

    [SerializeField] protected SpriteRenderer sprite;

    enum Rarity { Normall, Rare, Epic, Legend };


    bool isDroped;

    protected virtual void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(isDroped)
            transform.position = Input.mousePosition;
    }
}
