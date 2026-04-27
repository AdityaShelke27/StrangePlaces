using System.Linq;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public static ResourceHandler Instance;

    [SerializeField] Transform m_NavMeshParent;
    [SerializeField] InventorySlot[] m_Inventory = new InventorySlot[5];
    [SerializeField] LayerMask m_WorldPlacableLayer;
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
    }

    public void SelectItem(int idx)
    {
        if(idx < 0 || idx >= m_Inventory.Length) return;

        m_SelectedItemIdx = idx;
    }
    public void InstantiateObjectToWorld(StorableItem _item, Vector3 _pos)
    {
        GameObject obj = Instantiate(_item.GetWorldPrefab(), _pos, Quaternion.identity);
        obj.transform.parent = m_NavMeshParent;
        obj.GetComponent<WorldInstance>().Initialize(_item);
        NavMeshManager.s_BuildNavmesh?.Invoke();
    }
    public void InstantiateObjectToNodeWorld(StorableItem _item, Vector3 _pos, ResourceNodeInstance _node)
    {
        GameObject obj = Instantiate(_item.GetWorldPrefab(), _pos, Quaternion.identity);
        obj.transform.parent = m_NavMeshParent;
        obj.GetComponent<WorldInstance>().Initialize(_item);
        obj.GetComponent<NodeMachineInstance>().SetInputNode(_node);
        NavMeshManager.s_BuildNavmesh?.Invoke();
    }

    public (bool, Vector3, ResourceNodeInstance) CanPlaceWorld(StorableItem _item, Vector3 _pos)
    {
        if (_item == null) return (false, Vector3.zero, null);

        return _item.PlacementType switch
        {
            PlacementType.NodePlacement => CheckNodePlacement(_item, _pos),
            PlacementType.FreePlacement => CheckFreePlacement(_item, _pos),
            PlacementType.None => (false, Vector3.zero, null),
            _ => (false, Vector3.zero, null),
        };
    }

    (bool, Vector3, ResourceNodeInstance) CheckNodePlacement(StorableItem _item, Vector3 _pos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(_pos);
        Collider2D col = Physics2D.OverlapBox(worldPos, _item.Size, 0, m_WorldPlacableLayer);
        if (col && col.CompareTag("ResourceNode"))
        {
            ResourceNodeInstance m_Node = col.GetComponent<ResourceNodeInstance>();
            if(_item.GetPlacableNodes().Contains(m_Node.GetResourceNodeData()))
            {
                return (true, m_Node.transform.position, m_Node);
            }
            else
            {
                Debug.Log("Wrong Node");
            }
        }

        return (false, Vector3.zero, null);
    }
    (bool, Vector3, ResourceNodeInstance) CheckFreePlacement(StorableItem _item, Vector3 _pos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(_pos);
        Collider2D col = Physics2D.OverlapBox(worldPos, _item.Size, 0, m_WorldPlacableLayer);
        if (!col)
        {
            return (true, worldPos, null);
        }
        return (false, Vector3.zero, null);
    }
}
