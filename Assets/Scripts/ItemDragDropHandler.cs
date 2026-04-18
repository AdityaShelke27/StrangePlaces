using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragDropHandler : MonoBehaviour, IItemSlotUI, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    private static IItemSlotUI dragSource;
    private ItemSlot ItemSlot;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public bool CanAccept(ItemSlot item)
    {
        throw new System.NotImplementedException();
    }

    public ItemSlot GetItem()
    {
        return ItemSlot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragSource = GetComponent<IItemSlotUI>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(dragSource == null) return;

        Debug.Log($"Drag Source: {dragSource.GetItem()}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragSource = null;
    }

    public void SetItem(ItemSlot item)
    {
        throw new System.NotImplementedException();
    }
}
