using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bunker : MonoBehaviour, IActivate
{
	bool m_IsActivated = false;
	Animator m_Animator;

	private void Start()
	{
		m_Animator = GetComponent<Animator>();
	}
	private void OnMouseDown()
	{
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

		PlayerData.electricity = PlayerStatsManager.Instance.GetElectricity();
		PlayerData.hunger = PlayerStatsManager.Instance.GetHunger();
		PlayerData.researchPoints = PlayerStatsManager.Instance.GetResearchPoints();

		//PlayerData.isDataSaved = true;
		PlayerData.SaveData();
		SceneManager.LoadScene(Constant.SCENE_BUNKER);
	}

	public void Activate()
	{
		if (m_IsActivated) return;
		m_IsActivated = true;

		StartCoroutine(ActivateTime());
	}
	IEnumerator ActivateTime()
	{
		m_Animator.SetTrigger(Constant.BUNKER_OPEN);

		yield return new WaitForSeconds(1);

		MovePlayerToBunker();
	}
}
