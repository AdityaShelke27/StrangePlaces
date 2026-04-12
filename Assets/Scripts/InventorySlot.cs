using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ItemSlot m_ItemSlot;
    Image m_ItemImage;
    PointerEventData m_PointerData;
    bool m_IsDragging;
    bool CanPlace;
    Vector3 m_TargetPos;
    ResourceNodeInstance m_Node;

    void Start()
    {
        m_ItemImage = GetComponent<Image>();
        SetStorableItem(m_ItemSlot.item);
    }

    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /*if(m_ItemSlot.item is NodeMachineIns)
        {
            m_PointerData = eventData;
            m_IsDragging = true;
            StartCoroutine(StartCheckNodeMachine());
        }
        else if(m_ItemSlot.item is ResourceMachineIns)
        {
            
        }*/

        if (m_ItemSlot.item.PlacementType == PlacementType.None) return;

        m_IsDragging = true;
        m_PointerData = eventData;
        StartCoroutine(StartCheckPlacementPointer());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        /*if(m_ItemSlot.item is NodeMachineIns && m_Node)
        {
            Instantiate((m_ItemSlot.item as MachineIns).GetMachinePrefab(), m_Node.transform.position, Quaternion.identity);
        }
        m_PointerData = null;
        m_IsDragging = false;*/

        if(CanPlace)
        {
            if(m_ItemSlot.item.PlacementType == PlacementType.NodePlacement)
            {
                ResourceHandler.Instance.InstantiateObjectToNodeWorld(m_ItemSlot.item, m_TargetPos, m_Node);
            }
            else
            {
                ResourceHandler.Instance.InstantiateObjectToWorld(m_ItemSlot.item, m_TargetPos);
            }
        }

        m_PointerData = null;
        m_IsDragging = false;
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
}
