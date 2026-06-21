using System.Collections.Generic;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
	[SerializeField] BunkerPlayer m_Player;
	Dictionary<StorableItem, List<InventorySlot>> m_StoredItemDictionary = new();
	void Start()
	{
		CreateStoredItemDictionary();
	}

	private void CreateStoredItemDictionary()
	{
		Item[] _items = ItemDatabase.Instance.GetAllItems();

		foreach (Item item in _items) 
		{
			if(item is StorableItem)
			{
				m_StoredItemDictionary[item as StorableItem] = new();
			}
		}
		InventorySlot[] _playerInv = m_Player.GetPlayerInventory();

		foreach(InventorySlot _slot in _playerInv)
		{
			m_StoredItemDictionary[_slot.GetItem()].Add(_slot);
		}

		if (Storage.Instance == null) return;

		List<InventorySlot> _storageInv = Storage.Instance.GetStorageInventory();
		foreach (InventorySlot _slot in _storageInv)
		{
			m_StoredItemDictionary[_slot.GetItem()].Add(_slot);
		}
	}
}
