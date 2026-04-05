using System.Linq;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public static ResourceHandler Instance;

    [SerializeField] InventorySlot[] m_Inventory = new InventorySlot[5];
    [SerializeField] int m_SelectedItemIdx = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        m_SelectedItemIdx = -1;

        for (int i = 0; i < m_Inventory.Length; i++) 
        {
            //m_Inventory[i].SetStorableItem(new NodeMachineIns(m_MachinePrefabs.gameObject));
        }
    }

    public void SelectItem(int idx)
    {
        if(idx < 0 || idx >= m_Inventory.Length) return;

        m_SelectedItemIdx = idx;
    }

    public void NodeSelected(ResourceNodeInstance node)
    {
        /*if(m_SelectedItemIdx < 0)
        {
            Debug.Log("No Item Selected");
            return;
        }

        Debug.Log($"Working {m_Inventory[m_SelectedItemIdx].item.GetType()}");
        if (m_Inventory[m_SelectedItemIdx].item is MachineInstance)
        {   
            MachineInstance machine = m_Inventory[m_SelectedItemIdx].item as MachineInstance;
            if(machine.IsNodeInput())
            {
                if(machine.GetInputType() == node.GetNodeType())
                {
                    GameObject machineInstance = Instantiate(m_MachinePrefabs[machine.GetMachineID()].gameObject, node.transform.position, Quaternion.identity);
                    RemoveItemFromInventory(m_SelectedItemIdx);
                }
            }
        }*/
    }

    public void RemoveItemFromInventory(int itemIdx)
    {
        if(m_Inventory[itemIdx].GetItemSlot().amount > 0)
        {
            m_Inventory[itemIdx].GetItemSlot().amount--;
        }
            
        m_SelectedItemIdx = -1;
    }
    public void InstantiateObjectToWorld(StorableItem _item, Vector3 _pos)
    {
        GameObject obj = Instantiate(_item.GetWorldPrefab(), _pos, Quaternion.identity);
        obj.GetComponent<WorldInstance>().Initialize(_item);
    }

    public (bool, Vector3) CanPlaceWorld(StorableItem _item, Vector3 _pos)
    {
        switch (_item.PlacementType)
        {
            case PlacementType.NodePlacement:
                return CheckNodePlacement(_item, _pos);
            case PlacementType.FreePlacement:
                return CheckFreePlacement(_item, _pos);
            case PlacementType.None:
                return (false, Vector3.zero);
            default:
                return (false, Vector3.zero);
        }
    }

    (bool, Vector3) CheckNodePlacement(StorableItem _item, Vector3 _pos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(_pos);
        Collider2D col = Physics2D.OverlapPoint(worldPos);
        if (col && col.CompareTag("ResourceNode"))
        {
            ResourceNodeInstance m_Node = col.GetComponent<ResourceNodeInstance>();
            if(_item.GetPlacableNodes().Contains(m_Node.GetResourceNodeData()))
            {
                return (true, m_Node.transform.position);
            }
            Debug.Log("Working");
        }

        return (false, Vector3.zero);
    }
    (bool, Vector3) CheckFreePlacement(StorableItem _item, Vector3 _pos)
    {
        return (false, Vector3.zero);
    }
}
