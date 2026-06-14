using UnityEngine;
using UnityEngine.EventSystems;

public class Storage : MonoBehaviour
{
	[SerializeField] Transform m_InventoryParent;
	[SerializeField] GameObject m_InventorySlotPrefab;
	[SerializeField] int m_InventoryAmount;
	[SerializeField] GameObject m_InventoryPanelUI;
	private void Start()
	{
		CloseInventoryPanel();
		for (int i = 0; i < m_InventoryAmount; i++)
		{
			GameObject _invSlot = Instantiate(m_InventorySlotPrefab, m_InventoryParent);
			_invSlot.GetComponent<InventorySlot>().ShouldAcceptAllItems(true);
		}
	}
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		
		m_InventoryPanelUI.SetActive(true);
	}
	public void CloseInventoryPanel() => m_InventoryPanelUI.SetActive(false);
}
