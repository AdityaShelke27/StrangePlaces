using System;
using System.Collections;
using UnityEngine;

public class ResourceHandler : MonoBehaviour, IActivate
{
    public static ResourceHandler Instance;
	private Action PlaceMachine;

    [SerializeField] Transform m_NavMeshParent;
    [SerializeField] InventorySlot[] m_Inventory = new InventorySlot[5];
    [SerializeField] LayerMask m_WorldPlacableLayer;
	[SerializeField] float m_BuildTime;
	[SerializeField] float m_BuildDistance;
	[SerializeField] float m_WorkTime;
	[SerializeField] float m_WorkDistance;

	private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
	
	public InventorySlot[] GetInventorySlots() => m_Inventory;
    public void InstantiateObjectToWorld(StorableItem _item, Vector3 _pos, Action _action)
    {
		SurfaceMovement.Instance.MoveToKeepDistance(_pos, m_BuildDistance);
		//SurfaceMovement.Instance.MoveTo(_pos);
		PlaceMachine = () =>
		{
			SurfaceMovement.Instance.GetAnimator().SetBool(Constant.PLAYER_BUILD, true);
			StartCoroutine(DelayAction(m_BuildTime, () =>
			{
				SurfaceMovement.Instance.GetAnimator().SetBool(Constant.PLAYER_BUILD, false);
				GameObject obj = Instantiate(_item.GetWorldPrefab(), _pos, Quaternion.identity);
				obj.transform.parent = m_NavMeshParent;
				obj.GetComponent<WorldInstance>().Initialize(_item);
				_action?.Invoke();
				NavMeshManager.s_BuildNavmesh?.Invoke();
			}));
			
		};
		SurfaceMovement.s_Selected(gameObject);
	}
    public void InstantiateObjectToNodeWorld(StorableItem _item, Vector3 _pos, ResourceNodeInstance _node, Action _action)
    {
		SurfaceMovement.Instance.MoveToKeepDistance(_pos, m_BuildDistance);
		//SurfaceMovement.Instance.MoveTo(_pos);
		PlaceMachine = () => 
		{
			SurfaceMovement.Instance.GetAnimator().SetBool(Constant.PLAYER_BUILD, true);
			StartCoroutine(DelayAction(m_BuildTime, () =>
			{
				SurfaceMovement.Instance.GetAnimator().SetBool(Constant.PLAYER_BUILD, false);
				GameObject obj = Instantiate(_item.GetWorldPrefab(), _pos, Quaternion.identity);
				obj.transform.parent = m_NavMeshParent;
				obj.GetComponent<WorldInstance>().Initialize(_item);
				obj.GetComponent<NodeMachineInstance>().SetInputNode(_node);
				_action?.Invoke();
				NavMeshManager.s_BuildNavmesh?.Invoke();
			}));
				
		};
		SurfaceMovement.s_Selected(gameObject);
	}
	IEnumerator DelayAction(float _time, Action _action)
	{
		yield return new WaitForSeconds(_time);

		_action?.Invoke();
	}
	public void UseTool(Tool _tool, ResourceNodeInstance _node, Action _action)
	{
		int _amount = Mathf.Min(_node.GetAmountAvailable(), _tool.MineAmount);
		Resource _resource = _node.GetResourceNodeData().ResourceYield;
		if (ResourceTracker.Instance.IsItemAddable(_resource, _amount))
		{
			SurfaceMovement.Instance.MoveToKeepDistance(_node.transform.position, m_WorkDistance);
			PlaceMachine = () =>
			{
				SurfaceMovement.Instance.GetAnimator().SetBool(Constant.PLAYER_BUILD, true);
				int _amount = Mathf.Min(_node.GetAmountAvailable(), _tool.MineAmount);
				Resource _resource = _node.GetResourceNodeData().ResourceYield;
				StartCoroutine(DelayAction(m_WorkTime, () =>
				{
					SurfaceMovement.Instance.GetAnimator().SetBool(Constant.PLAYER_BUILD, false);
					ResourceTracker.Instance.AddStorableItemToInventory(_resource, _node.FetchResource(_tool.MineAmount));
					_action?.Invoke();
					//AddItemAmount(-1);
				}));
			};
			SurfaceMovement.s_Selected(gameObject);
		}
		else Debug.LogWarning("Not enough space in inventory");
	}
    public (bool, Vector3, ResourceNodeInstance) CanPlaceWorld(StorableItem _item, Vector3 _pos)
    {
        if (_item == null) return (false, Vector3.zero, null);

        return _item.PlacementType switch
        {
            E_PlacementType.NodePlacement => CheckNodePlacement(_item, _pos),
            E_PlacementType.FreePlacement => CheckFreePlacement(_item, _pos),
            E_PlacementType.None => (false, Vector3.zero, null),
            _ => (false, Vector3.zero, null),
        };
    }

    (bool, Vector3, ResourceNodeInstance) CheckNodePlacement(StorableItem _item, Vector3 _pos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(_pos);
        Collider2D col = Physics2D.OverlapBox(worldPos, _item.Size, 0, m_WorldPlacableLayer);
        if (col && col.CompareTag(Constant.TAG_RESOURCE_NODE))
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

	public void Activate()
	{
		PlaceMachine?.Invoke();
	}
}
