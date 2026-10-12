using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour
{
	private void OnMouseUpAsButton()
	{
		if (InputManager.IsADrag()) return;

		StartCoroutine(Constant.DelayExecute(() =>
		{
			if (InputManager.IsPointerOverUI()) return;

			InventorySlot[] _inventory = ResourceTracker.Instance.GetPlayerInventory();

			for (int i = 0; i < _inventory.Length; i++)
			{
				InventorySlot _inv = _inventory[i];
				PlayerData.itemSlot[i] = new(_inv.GetItem(), _inv.GetItemAmount());
			}

			PlayerData.electricity = PlayerStatsManager.Instance.GetElectricity();
			PlayerData.hunger = PlayerStatsManager.Instance.GetHunger();
			PlayerData.researchPoints = PlayerStatsManager.Instance.GetResearchPoints();

			PlayerData.SaveData();

			PlayerPrefs.SetInt(Constant.PREF_DAY, PlayerPrefs.GetInt(Constant.PREF_DAY) + 1);
			SceneManager.LoadScene(Constant.SCENE_SURFACE);
		}));
	}
}
