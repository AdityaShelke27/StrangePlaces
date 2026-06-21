using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Storage : MonoBehaviour
{
	public static Storage Instance;

	[SerializeField] Transform m_InventoryParent;
	[SerializeField] GameObject m_InventorySlotPrefab;
	[SerializeField] int m_InventoryAmount;
	[SerializeField] GameObject m_InventoryPanelUI;
	readonly List<InventorySlot> m_StorageInventory = new();

	private void Awake()
	{
		if(Instance == null) Instance = this;
		else Destroy(gameObject);
	}
	private void Start()
	{
		ClosePanel();
		for (int i = 0; i < m_InventoryAmount; i++)
		{
			GameObject _invSlot = Instantiate(m_InventorySlotPrefab, m_InventoryParent);
			InventorySlot _slot = _invSlot.GetComponent<InventorySlot>();
			_slot.ShouldAcceptAllItems(true);
			m_StorageInventory.Add(_slot);
		}
	}
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		
		m_InventoryPanelUI.SetActive(true);
	}
	public List<InventorySlot> GetStorageInventory() => m_StorageInventory;
	public void ClosePanel() => m_InventoryPanelUI.SetActive(false);
}
