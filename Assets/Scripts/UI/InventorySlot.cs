using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	[SerializeField] ItemSlot m_ItemSlot;
	[SerializeField] Image m_ItemImage;
	[SerializeField] TMP_Text m_ItemAmountText;
	[SerializeField] List<StorableItem> m_IncludeItems;
	[SerializeField] bool m_ShouldAcceptAllItems;
	PointerEventData m_PointerData;
	bool m_IsDragging;
	bool CanPlace;
	Vector3 m_TargetPos;
	ResourceNodeInstance m_Node;

	public static InventorySlot s_SourceInventorySlot;

	void Start()
	{
		SetItemSlot(m_ItemSlot.item, m_ItemSlot.amount);
	}

	IEnumerator StartCheckPlacementPointer()
	{
		if (ResourceHandler.Instance == null) yield break;
		while (m_IsDragging)
		{
			(CanPlace, m_TargetPos, m_Node) = ResourceHandler.Instance.CanPlaceWorld(m_ItemSlot.item, m_PointerData.position);

			yield return null;
		}
	}
	public void SetItemSlot(StorableItem _item, int _amount)
	{
		m_ItemSlot.item = _item;
		m_ItemSlot.amount = _amount;

		if (_item == null)
		{
			m_ItemImage.enabled = false;
			m_ItemAmountText.enabled = false;
		}
		else
		{
			m_ItemImage.enabled = true;
			m_ItemAmountText.enabled = true;
			m_ItemImage.sprite = _item.itemImage;
			m_ItemAmountText.text = _amount.ToString();
		}
	}
	public StorableItem GetItem() => m_ItemSlot.item;
	public int GetItemAmount() => m_ItemSlot.amount;
	public void OnBeginDrag(PointerEventData eventData)
	{
		m_IsDragging = true;
		m_PointerData = eventData;
		s_SourceInventorySlot = this;
		StartCoroutine(StartCheckPlacementPointer());
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if(eventData.pointerEnter != null) return;
		if (CanPlace)
		{
			if (m_ItemSlot.item.PlacementType == E_PlacementType.NodePlacement)
			{
				if(ItemDatabase.Instance.DoesItemIDExistInResearch(m_Node.GetResourceNodeData().itemID))
				{
					ResourceHandler.Instance.InstantiateObjectToNodeWorld(m_ItemSlot.item, m_TargetPos, m_Node);
				}
				else
				{
					Debug.LogWarning("Item not researched yet");
					m_IsDragging = false;
					return;
				}
			}
			else if(m_ItemSlot.item.PlacementType == E_PlacementType.FreePlacement)
			{
				ResourceHandler.Instance.InstantiateObjectToWorld(m_ItemSlot.item, m_TargetPos);
			}

			ResetSourceInventorySlot();
		}
		s_SourceInventorySlot = null;
		m_PointerData = null;
		m_IsDragging = false;
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (s_SourceInventorySlot == null) return;
		if(!CanAcceptItem(s_SourceInventorySlot.GetItem())) return;
		Debug.Log($"ID: {s_SourceInventorySlot.GetItem().itemID}, {ItemDatabase.Instance.DoesItemIDExistInResearch(s_SourceInventorySlot.GetItem().itemID)}");
		if (!ItemDatabase.Instance.DoesItemIDExistInResearch(s_SourceInventorySlot.GetItem().itemID)) return;

		if(GetItem() == s_SourceInventorySlot.GetItem())
		{
			int _sumAmount = GetItemAmount() + s_SourceInventorySlot.GetItemAmount();
			if(_sumAmount > GetItem().StackableAmount)
			{
				int _addAmount = GetItem().StackableAmount - GetItemAmount();
				AddItemAmount(_addAmount);
				s_SourceInventorySlot.AddItemAmount(-_addAmount);
			}
			else
			{
				SetItemAmount(_sumAmount);
				ResetSourceInventorySlot();
			}
		}
		else if(GetItem() != null)
		{
			return;
		}
		else
		{
			SetItemSlot(s_SourceInventorySlot.GetItem(), s_SourceInventorySlot.GetItemAmount());
			ResetSourceInventorySlot();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		
	}

	bool CanAcceptItem(StorableItem _item)
	{
		if(m_ShouldAcceptAllItems) return true;

		for (int i = 0; i < m_IncludeItems.Count; i++)
		{
			if(_item == m_IncludeItems[i]) return true;
		}

		return false;
	}

	public void AddIncludeItems(StorableItem _item)
	{
		m_IncludeItems.Add(_item);
	}

	public void AddIncludeItems(List<StorableItem> _items)
	{
		m_IncludeItems.AddRange(_items);
	}
	public void SetItemAmount(int _amount)
	{
		if(_amount < 0)
		{
			Debug.Log("Inventory slot item amount cant be negative");
			return;
		}

		m_ItemSlot.amount = _amount;
		m_ItemAmountText.text = _amount.ToString();
	}
	public void AddItemAmount(int _amount)
	{
		if (m_ItemSlot.amount + _amount < 0)
		{
			Debug.Log("Inventory slot item amount cant be negative");
			return;
		}

		m_ItemSlot.amount += _amount;
		m_ItemAmountText.text = m_ItemSlot.amount.ToString();
	}
	public void SetItem(StorableItem _item)
	{
		m_ItemSlot.item = _item;
		if (_item == null)
		{
			m_ItemImage.enabled = false;
			m_ItemAmountText.enabled = false;
		}
		else
		{
			m_ItemImage.enabled = true;
			m_ItemAmountText.enabled = true;
			m_ItemImage.sprite = _item.itemImage;
		}
	}
	public void RemoveItemFromInventory()
	{
		SetItemSlot(null, 0);
	}

	public void ShouldAcceptAllItems(bool _val) => m_ShouldAcceptAllItems = _val;

	public static void ResetSourceInventorySlot()
	{
	s_SourceInventorySlot.RemoveItemFromInventory();
		s_SourceInventorySlot = null;
	}
}
