using UnityEngine;
using UnityEngine.SceneManagement;

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

		for (int i = 0; i < _inventory.Length; i++)
		{
			InventorySlot _inv = _inventory[i];
			PlayerData.itemSlot[i] = new(_inv.GetItem(), _inv.GetItemAmount());
		}
		PlayerData.isDataSaved = true;
		SceneManager.LoadScene(Constant.SCENE_BUNKER);
	}

}
