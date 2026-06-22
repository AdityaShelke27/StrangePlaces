using System.Collections.Generic;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
	public static ResourceTracker Instance;

	[SerializeField] BunkerPlayer m_Player;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else if(Instance != this)
		{
			Destroy(gameObject);
		}
	}
	public bool SearchResourceAvailable(StorableItem _item, int _amount)
	{
		InventorySlot[] _playerInventory = m_Player.GetPlayerInventory();
		int _searchedAmount = 0;

		for(int i = 0; i < _playerInventory.Length; i++)
		{
			if (_playerInventory[i].GetItem() != _item) continue;

			_searchedAmount += _playerInventory[i].GetItemAmount();
		}

		return _searchedAmount >= _amount;
	}

	public bool SearchAndRemoveResource(StorableItem _item, int _amount)
	{
		InventorySlot[] _playerInventory = m_Player.GetPlayerInventory();
		int _searchedAmount = 0;
		List<int> _itemIdx = new();

		for (int i = 0; i < _playerInventory.Length; i++)
		{
			if (_playerInventory[i].GetItem() != _item) continue;

			_searchedAmount += _playerInventory[i].GetItemAmount();
			_itemIdx.Add(i);
		}

		if(_searchedAmount >= _amount)
		{
			int _requiredAmount = _amount;
			for(int i = 0; i < _itemIdx.Count; i++)
			{
				InventorySlot _selectedSlot = _playerInventory[_itemIdx[i]];
				if (_selectedSlot.GetItemAmount() <= _requiredAmount)
				{
					_requiredAmount -= _selectedSlot.GetItemAmount();
					_selectedSlot.RemoveItemFromInventory();
				}
				else
				{
					_selectedSlot.AddItemAmount(-_requiredAmount);
					_requiredAmount = 0;
				}
			}
			return true;
		}
		else return false;
	}
}
