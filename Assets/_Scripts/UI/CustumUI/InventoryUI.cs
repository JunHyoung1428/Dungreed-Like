using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] public bool isActivated;

    [SerializeField] public GameObject inventoryBase;
    [SerializeField] GameObject slotsGrid;

    InventorySlot[] slots;
    void Start()
    {
        slots = slotsGrid.GetComponentsInChildren<InventorySlot>();
    }


    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    private InventorySlot _beginDragSlot; // ���� �巡�׸� ������ ����
    private Transform _beginDragIconTransform; // �ش� ������ ������ Ʈ������

    private Vector3 _beginDragIconPoint;   // �巡�� ���� �� ������ ��ġ
    private Vector3 _beginDragCursorPoint; // �巡�� ���� �� Ŀ���� ��ġ
    private int _beginDragSlotSiblingIndex;

    private void Update()
    {
       // _ped.position = Input.mousePosition;

       // OnPointerDown();
       // OnPointerDrag();
       // OnPointerUp();
    }

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();

        _gr.Raycast(_ped, _rrList);

        if (_rrList.Count == 0)
            return null;

        return _rrList[0].gameObject.GetComponent<T>();
    }

    private void OnPointerDown()
    {
        // Left Click : Begin Drag
        if (Input.GetMouseButtonDown(0))
        {
            _beginDragSlot = RaycastAndGetFirstComponent<InventorySlot>();

            // �������� ���� �ִ� ���Ը� �ش�
            if (_beginDragSlot != null && _beginDragSlot.HasItem)
            {
                // ��ġ ���, ���� ���
                _beginDragIconTransform = _beginDragSlot.IconRect.transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;

                // �� ���� ���̱�
                _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                _beginDragSlot.transform.SetAsLastSibling();

                // �ش� ������ ���̶���Ʈ �̹����� �����ܺ��� �ڿ� ��ġ��Ű��
                //_beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                _beginDragSlot = null;
            }
        }
    }
    /// <summary> �巡���ϴ� ���� </summary>
    private void OnPointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            // ��ġ �̵�
            _beginDragIconTransform.position =
                _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
        }
    }
    /// <summary> Ŭ���� �� ��� </summary>
    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // End Drag
            if (_beginDragSlot != null)
            {
                // ��ġ ����
                _beginDragIconTransform.position = _beginDragIconPoint;

                // UI ���� ����
                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                // �巡�� �Ϸ� ó��
                EndDrag();

                // ���� ����
                _beginDragSlot = null;
                _beginDragIconTransform = null;
            }
        }
    }


    private void EndDrag()
    {
        InventorySlot endDragSlot = RaycastAndGetFirstComponent<InventorySlot>();

        if (endDragSlot != null && endDragSlot.IsAccessible)
        {
            TrySwapItems(_beginDragSlot, endDragSlot);
        }
    }

    /// <summary> �� ������ ������ ��ȯ </summary>
    private void TrySwapItems(InventorySlot from, InventorySlot to)
    {
        if (from == to)
        {
            return;
        }

        from.SwapOrMoveIcon(to);
        //_inventory.Swap(from.Index, to.Index);
    }
}
