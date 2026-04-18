using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] ItemSlot m_ItemSlot;
    [SerializeField] Image m_ItemImage;
    [SerializeField] TMP_Text m_ItemAmount;
    [SerializeField] MonoScript[] m_IncludeItems;
    [SerializeField] MonoScript[] m_ExcludeItems;
    PointerEventData m_PointerData;
    bool m_IsDragging;
    bool CanPlace;
    Vector3 m_TargetPos;
    ResourceNodeInstance m_Node;

    public static ItemSlot s_SourceInventorySlot;

    void Start()
    {
        SetStorableItem(m_ItemSlot.item);
    }

    IEnumerator StartCheckPlacementPointer()
    {
        while (m_IsDragging)
        {
            (CanPlace, m_TargetPos, m_Node) = ResourceHandler.Instance.CanPlaceWorld(m_ItemSlot.item, m_PointerData.position);

            yield return null;
        }
    }

    public void SetStorableItem(StorableItem _item)
    {
        m_ItemSlot.item = _item;
        m_ItemImage.sprite = m_ItemSlot.item.itemImage;
    }
    public ItemSlot GetItemSlot() => m_ItemSlot;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (m_ItemSlot.item.PlacementType == PlacementType.None) return;
        Debug.Log("Begin Drag");
        m_IsDragging = true;
        m_PointerData = eventData;
        s_SourceInventorySlot = m_ItemSlot;
        StartCoroutine(StartCheckPlacementPointer());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(eventData.pointerEnter != null) return;
        Debug.Log("End Drag");
        if (CanPlace)
        {
            if (m_ItemSlot.item.PlacementType == PlacementType.NodePlacement)
            {
                ResourceHandler.Instance.InstantiateObjectToNodeWorld(m_ItemSlot.item, m_TargetPos, m_Node);
            }
            else if(m_ItemSlot.item.PlacementType == PlacementType.FreePlacement)
            {
                ResourceHandler.Instance.InstantiateObjectToWorld(m_ItemSlot.item, m_TargetPos);
            }
        }
        s_SourceInventorySlot = null;
        m_PointerData = null;
        m_IsDragging = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (s_SourceInventorySlot == null) return;
        Debug.Log("Drop");
        if(!CanAcceptItem(s_SourceInventorySlot.item)) return;

        SetStorableItem(s_SourceInventorySlot.item);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
    }

    bool CanAcceptItem(StorableItem _item)
    {
        for(int i = 0; i < m_ExcludeItems.Length; i++)
        {
            System.Type type = m_ExcludeItems[i].GetClass();

            if (type.IsAssignableFrom(_item.GetType())) return false;
        }

        for (int i = 0; i < m_IncludeItems.Length; i++)
        {
            System.Type type = m_IncludeItems[i].GetClass();

            if (type.IsAssignableFrom(_item.GetType())) return true;
        }

        return false;
    }
}
