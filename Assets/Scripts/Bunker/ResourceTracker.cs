using System.Collections.Generic;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
	public static ResourceTracker Instance;

	[SerializeField] BunkerPlayer m_Player;
	InventorySlot[] m_PlayerInventory;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}
	private void Start()
	{
		m_PlayerInventory = m_Player.GetPlayerInventory();
	}
	public bool SearchResourceAvailable(StorableItem _item, int _amount)
	{
		int _searchedAmount = 0;

		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() != _item) continue;

			_searchedAmount += m_PlayerInventory[i].GetItemAmount();
		}

		return _searchedAmount >= _amount;
	}

	public bool SearchAndRemoveResource(StorableItem _item, int _amount)
	{
		int _searchedAmount = 0;
		List<int> _itemIdx = new();

		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() != _item) continue;

			_searchedAmount += m_PlayerInventory[i].GetItemAmount();
			_itemIdx.Add(i);
		}

		if (_searchedAmount >= _amount)
		{
			int _requiredAmount = _amount;
			for (int i = 0; i < _itemIdx.Count; i++)
			{
				InventorySlot _selectedSlot = m_PlayerInventory[_itemIdx[i]];
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
	public int GetEmptyInventorySlots()
	{
		int _availableSlots = 0;
		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() == null) _availableSlots++;
		}

		return _availableSlots;
	}
	public bool IsItemAddable(StorableItem _item, int _amount)
	{
		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() == null && _amount <= _item.StackableAmount) return true;

			if(m_PlayerInventory[i].GetItem() == _item && m_PlayerInventory[i].GetItemAmount() + _amount <= _item.StackableAmount) return true;
		}

		return false;
	}
	public void AddStorableItemToInventory(StorableItem _item, int _amount)
	{
		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() == null)
			{
				m_PlayerInventory[i].SetItemSlot(_item, _amount);
				break;
			}
			else if(m_PlayerInventory[i].GetItem() == _item && m_PlayerInventory[i].GetItemAmount() + _amount <= _item.StackableAmount)
			{
				m_PlayerInventory[i].AddItemAmount(_amount);
				break;
			}
		}
	}
}
