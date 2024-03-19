using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour
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
        
    }

}
