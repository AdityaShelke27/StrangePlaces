using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
	public static ResourceTracker Instance;
	public static Action<ResourceNodeInstance> s_ShowNodeDetails;

	[SerializeField] GameObject m_DetailPanel;
	[SerializeField] TMP_Text m_NameText;
	[SerializeField] InventorySlot[] m_PlayerInventory;

	bool m_IsShowingPanel = false;

	private void OnEnable()
	{
		s_ShowNodeDetails += SetNodeDetails;
	}
	private void OnDisable()
	{
		s_ShowNodeDetails -= SetNodeDetails;
	}

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
		m_DetailPanel.SetActive(false);
	}
	void SetNodeDetails(ResourceNodeInstance _node)
	{
		if (m_IsShowingPanel) return;

		StartCoroutine(ShowNode(_node));
	}
	IEnumerator ShowNode(ResourceNodeInstance _node)
	{
		m_DetailPanel.transform.position = _node.gameObject.transform.position + Vector3.up;
		ResourceNode _data = _node.GetResourceNodeData();
		m_NameText.text = ItemDatabase.Instance.DoesItemIDExistInResearch(_data.itemID) ? _data.itemName : "???";
		m_DetailPanel.SetActive(true);
		m_IsShowingPanel = true;

		yield return new WaitForSeconds(2);

		m_DetailPanel.SetActive(false);
		m_IsShowingPanel = false;
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
	public int SearchResourceAvailableAmount(StorableItem _item, int _amount)
	{
		int _searchedAmount = 0;

		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() != _item) continue;

			_searchedAmount += m_PlayerInventory[i].GetItemAmount();
		}

		return _searchedAmount;
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
	public int SearchAndRemoveMaxAmountResource(StorableItem _item, int _amount)
	{
		int _requiredAmount = _amount;

		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() != _item) continue;

			if(m_PlayerInventory[i].GetItemAmount() > _requiredAmount)
			{
				m_PlayerInventory[i].SetItemAmount(m_PlayerInventory[i].GetItemAmount() - _requiredAmount);

				return _amount;
			}
			else
			{
				_requiredAmount -= m_PlayerInventory[i].GetItemAmount();
				m_PlayerInventory[i].RemoveItemFromInventory();
			}
		}

		return _amount - _requiredAmount;
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
		int _amountToAdd = _amount;
		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() == null) _amountToAdd -= _item.StackableAmount;
			else if (m_PlayerInventory[i].GetItem() == _item)
			{
				_amountToAdd -= _item.StackableAmount - m_PlayerInventory[i].GetItemAmount();
			}

			if(_amountToAdd <= 0) return true;
		}

		return false;
	}
	public void AddStorableItemToInventory(StorableItem _item, int _amount)
	{
		int _amountToAdd = _amount;
		for (int i = 0; i < m_PlayerInventory.Length; i++)
		{
			if (m_PlayerInventory[i].GetItem() == null)
			{
				int _added = Mathf.Min(_amountToAdd, _item.StackableAmount);
				m_PlayerInventory[i].SetItemSlot(_item, _added);
				_amountToAdd -= _added;
			}
			else if(m_PlayerInventory[i].GetItem() == _item)
			{
				int _added = Mathf.Min(_amountToAdd, _item.StackableAmount - m_PlayerInventory[i].GetItemAmount());
				m_PlayerInventory[i].AddItemAmount(_added);
				_amountToAdd -= _added;
			}
			if (_amountToAdd == 0) return;
			else if(_amountToAdd < 0)
			{
				Debug.LogWarning($"Something went wrong while adding item: {m_PlayerInventory[i].GetItem().itemName}");
				return;
			}
		}

		if(_amountToAdd > 0)
		{
			Debug.LogWarning($"Remaining amount cannot be added: {_amountToAdd} for item {_item.itemName}");
		}
	}
}
