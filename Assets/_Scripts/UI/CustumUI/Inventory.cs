using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] public bool isActivated;

    [SerializeField] GameObject inventoryBase;
    [SerializeField] GameObject slotsGrid;

    InventorySlot[] slots;
    void Start()
    {
        slots = slotsGrid.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }


    void OpenInventory()
    {
        inventoryBase.SetActive(true);
    }
    void CloseInventory()
    {
        inventoryBase.SetActive(false);
    }

}
