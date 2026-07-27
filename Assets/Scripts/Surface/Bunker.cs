using UnityEngine;
using UnityEngine.SceneManagement;

public class Bunker : MonoBehaviour, IActivate
{
	bool m_IsActivated = false;
	private void OnMouseDown()
	{
		Debug.Log("Pressing");
		SurfaceMovement.s_Selected?.Invoke(gameObject);
	}
	//private void OnTriggerEnter2D(Collider2D collision)
	//{
	//	if(collision.CompareTag(Constant.TAG_PLAYER))
	//	{
	//		MovePlayerToBunker();
	//	}
	//}
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

	public void Activate()
	{
		if (m_IsActivated) return;

		MovePlayerToBunker();
		m_IsActivated = true;
	}
}
