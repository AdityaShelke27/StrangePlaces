using System.Collections;
using UnityEngine;

public class ResourceNodeInstance : MonoBehaviour
{
	[SerializeField] SpriteRenderer m_SpriteRenderer;
	[SerializeField] Material m_WindDeformMat;
	[SerializeField] int m_AmountAvailable;
	ResourceNode m_ResourceNodeData;
	int m_MaxAmount;
	bool m_AllResourcesDepleted = false;

	void Start()
	{
		Initialize();
	}
	private void OnMouseUpAsButton()
	{
		if (InputManager.IsADrag()) return;

		StartCoroutine(Constant.DelayExecute(() =>
		{
			if (InputManager.IsPointerOverUI()) return;
			ResourceTracker.s_ShowNodeDetails?.Invoke(this);
		}));
	}
	void Initialize()
	{
		m_SpriteRenderer.sprite = m_ResourceNodeData.itemImage;
		m_MaxAmount = m_ResourceNodeData.MaxAmount;
		m_AmountAvailable = m_MaxAmount;

		if (m_ResourceNodeData.NodeType == E_SurfaceNode.Plant_Node)
		{
			m_SpriteRenderer.material = m_WindDeformMat;
		}
	}
	public int FetchResource(int amount)
	{
		if (m_AllResourcesDepleted) return 0;

		if(amount < m_AmountAvailable)
		{
			m_AmountAvailable -= amount;
		}
		else
		{
			amount = m_AmountAvailable;
			m_AmountAvailable = 0;
			m_AllResourcesDepleted = true;
			StartCoroutine(DestroyNodeNextFrame());
		}

		return amount;
	}
	IEnumerator DestroyNodeNextFrame()
	{
		yield return null;

		Destroy(gameObject);
	}
	public ResourceNode GetResourceNodeData() => m_ResourceNodeData;
	public void SetResourceNodeData(ResourceNode _nodeData)
	{
		m_ResourceNodeData = _nodeData;
		Initialize();
	}
	public int GetAmountAvailable() => m_AmountAvailable;
}
