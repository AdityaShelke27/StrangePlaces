using UnityEngine;

public class BunkerPlayer : MonoBehaviour
{
	[SerializeField] InventorySlot[] m_InventorySlots;
	void Start()
	{
		AssignInventory();
	}
	void AssignInventory()
	{
		if (!PlayerData.isDataSaved) return;

		PlayerData.isDataSaved = false;

		ItemSlot[] _items = PlayerData.itemSlot;
		for (int i = 0; i < _items.Length; i++)
		{
			m_InventorySlots[i].SetItemSlot(_items[i].item, _items[i].amount);
		}
	}

	public InventorySlot[] GetPlayerInventory() => m_InventorySlots;
}
