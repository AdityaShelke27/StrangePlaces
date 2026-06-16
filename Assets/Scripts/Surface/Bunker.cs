using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class Bunker : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag(Constant.TAG_PLAYER))
		{
			MovePlayerToBunker();
		}
	}
	void MovePlayerToBunker()
	{
		InventorySlot[] _inventory = ResourceHandler.Instance.GetInventorySlots();
		Save_ItemSlot[] _saveItems = new Save_ItemSlot[_inventory.Length];

		for(int i = 0; i < _inventory.Length; i++)
		{
			InventorySlot _inv = _inventory[i];
			StorableItem _item = _inv.GetItem();

			_saveItems[i] = _item != null ? new(_item.itemID, _inv.GetItemAmount()) : new("", 0);
		}

		PlayerPrefs.SetString(Constant.PREF_SAVE_INVENTORY, JsonConvert.SerializeObject(_saveItems));

		SceneManager.LoadScene(Constant.SCENE_BUNKER);
	}
}
