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
	List<InventorySlot> m_StorageInventory = new();

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
		StartCoroutine(Constant.DelayExecute(() =>
		{
			if (EventSystem.current.IsPointerOverGameObject()) return;

			m_InventoryPanelUI.SetActive(true);
		}));
	}
	public List<InventorySlot> GetStorageInventory() => m_StorageInventory;
	public void ClosePanel() => m_InventoryPanelUI.SetActive(false);

	public void SaveStorage()
	{
		Save_Inventory _saveInv = new()
		{
			itemSlot = new Save_ItemSlotArray[m_StorageInventory.Count]
		};

		for (int i = 0; i < m_StorageInventory.Count; i++)
		{
			StorableItem _item = m_StorageInventory[i].GetItem();
			_saveInv.itemSlot[i] = _item != null ? new(_item.itemID, m_StorageInventory[i].GetItemAmount()) : new("", 0);
		}

		Save_Inventory.SaveData(_saveInv);
	}

	public void LoadStorage()
	{
		Save_ItemSlotArray[] _itemArray = Save_Inventory.LoadData();
		if (_itemArray != null) return;

		for (int i = 0; i < _itemArray.Length; i++)
		{
			if(string.IsNullOrEmpty(_itemArray[i].id))
			{
				m_StorageInventory[i].SetItemSlot(null, 0);
			}
			else
			{
				m_StorageInventory[i].SetItemSlot(ItemDatabase.Instance.GetItemByID(_itemArray[i].id) as StorableItem, _itemArray[i].amount);
			}
		}
	}
}
