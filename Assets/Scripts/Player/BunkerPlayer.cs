using UnityEngine;

public class BunkerPlayer : MonoBehaviour
{
	[SerializeField] InventorySlot[] m_InventorySlots;
	private void Awake()
	{
		QualitySettings.vSyncCount = 0;
		// Dynamically match the screen's refresh rate (e.g., 60, 90, 120)
		Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
	}
	void Start()
	{
		AssignInventory();
	}
	void AssignInventory()
	{
		//if (!PlayerData.isDataSaved) return;

		//PlayerData.isDataSaved = false;

		PlayerData.LoadData();

		ItemSlot[] _items = PlayerData.itemSlot;
		for (int i = 0; i < _items.Length; i++)
		{
			m_InventorySlots[i].SetItemSlot(_items[i].item, _items[i].amount);
		}

		PlayerStatsManager.Instance.SetElectricity(PlayerData.electricity);
		PlayerStatsManager.Instance.SetHunger(PlayerData.hunger);
		PlayerStatsManager.Instance.SetResearchPoints(PlayerData.researchPoints);
	}

	public InventorySlot[] GetPlayerInventory() => m_InventorySlots;
}
