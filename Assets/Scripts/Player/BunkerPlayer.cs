using Newtonsoft.Json;
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
		if (!PlayerPrefs.HasKey(Constant.PREF_SAVE_INVENTORY)) return;

		Save_ItemSlot[] _items = JsonConvert.DeserializeObject<Save_ItemSlot[]>(PlayerPrefs.GetString(Constant.PREF_SAVE_INVENTORY));

		for(int i = 0; i < _items.Length; i++)
		{
			if (string.IsNullOrEmpty(_items[i].id)) continue;

			m_InventorySlots[i].SetItemSlot(ItemDatabase.Instance.GetItemByID(_items[i].id) as StorableItem, _items[i].amount);
		}
		PlayerPrefs.DeleteKey(Constant.PREF_SAVE_INVENTORY);
	}
}
