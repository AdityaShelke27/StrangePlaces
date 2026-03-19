using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public static ResourceHandler Instance;

    [SerializeField] Item[] m_Inventory = new Item[5];
    [SerializeField] Item m_SelectedItem;
    [SerializeField] Machine[] m_MachinePrefabs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectItem(int idx)
    {
        if(idx < 0 || idx >= m_Inventory.Length) return;

        m_SelectedItem = m_Inventory[idx];
    }

    public void NodeSelected(ResourceNode node)
    {
        if(m_SelectedItem is Machine)
        {
            Machine machine = m_SelectedItem as Machine;
            if(machine.IsNodeInput())
            {
                if(machine.GetInputType() == node.GetNodeType())
                {
                    GameObject machineInstance = Instantiate(m_MachinePrefabs[machine.GetMachineID()].gameObject, node.transform.position, Quaternion.identity);
                }
            }
        }
    }
}
