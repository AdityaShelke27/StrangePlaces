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
		PlayerData.LoadData();

		ItemSlot[] _items = PlayerData.itemSlot;
		for (int i = 0; i < _items.Length; i++)
		{
			if (_items[i] == null) continue;

			m_InventorySlots[i].SetItemSlot(_items[i].item, _items[i].amount);
		}

		PlayerStatsManager.Instance.SetElectricity(PlayerData.electricity);
		PlayerStatsManager.Instance.SetHunger(PlayerData.hunger);
		PlayerStatsManager.Instance.SetResearchPoints(PlayerData.researchPoints);
	}

	public InventorySlot[] GetPlayerInventory() => m_InventorySlots;
}
