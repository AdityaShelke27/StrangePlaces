using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ItemSlot m_ItemSlot;
    PointerEventData m_PointerData;
    bool m_IsDragging;
    bool CanPlace;
    Vector3 m_TargetPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
        StartCoroutine(StartCheckNodeMachine());
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
            ResourceHandler.Instance.InstantiateObjectToWorld(m_ItemSlot.item, m_TargetPos);
        }

        m_PointerData = null;
        m_IsDragging = false;
    }

    IEnumerator StartCheckNodeMachine()
    {
        while (m_IsDragging)
        {
            (CanPlace, m_TargetPos) = ResourceHandler.Instance.CanPlaceWorld(m_ItemSlot.item, m_PointerData.position);

            yield return null;
        }
        
    }
    IEnumerator StartCheckResourceMachine()
    {
        yield return null;
    }

    public void SetStorableItem(StorableItem _item)
    {
        m_ItemSlot.item = _item;
    }
    public ItemSlot GetItemSlot() => m_ItemSlot;
}
