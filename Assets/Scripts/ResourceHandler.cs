using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public static ResourceHandler Instance;

    [SerializeField] ItemSlot[] m_Inventory = new ItemSlot[5];
    [SerializeField] int m_SelectedItemIdx = -1;
    [SerializeField] Machine[] m_MachinePrefabs;

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
    }

    public void SelectItem(int idx)
    {
        if(idx < 0 || idx >= m_Inventory.Length) return;

        m_SelectedItemIdx = idx;
    }

    public void NodeSelected(ResourceNodeTemp node)
    {
        /*if(m_SelectedItemIdx < 0)
        {
            Debug.Log("No Item Selected");
            return;
        }

        if (m_Inventory[m_SelectedItemIdx].item is Machine)
        {   
            Machine machine = m_Inventory[m_SelectedItemIdx].item as Machine;
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
        if(m_Inventory[itemIdx].amount > 1)
        {
            m_Inventory[itemIdx].amount--;
        }
        else
        {
            m_Inventory[itemIdx] = null;
        }
            
        m_SelectedItemIdx = -1;
    }
}
