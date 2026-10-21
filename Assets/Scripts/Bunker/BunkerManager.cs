using System.Collections;
using TMPro;
using UnityEngine;

public class BunkerManager : MonoBehaviour
{
	public static BunkerManager Instance;

	[SerializeField] private GameObject m_TutorialManager;
	[SerializeField] private GameObject m_ObjectivePanel;
	[SerializeField] private TMP_Text m_DayText;
	[SerializeField] private float m_InventorySaveFrequency = 5;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		if (PlayerPrefs.GetInt(Constant.PREF_TUTORIAL) == 1)
		{
			Instantiate(m_TutorialManager);
		}
	}
	private void Start()
	{
		m_ObjectivePanel.SetActive(false);

		TutorialManager.s_Trigger?.Invoke("0");
		m_DayText.text = $"DAY {PlayerPrefs.GetInt(Constant.PREF_DAY)}";

		StartCoroutine(SavePlayerInventoryRecur());
	}

	public void ShowObjectivePanel()
	{
		m_ObjectivePanel.SetActive(true);
		AudioManager.Instance.PlayButtonClick();
	}

	public void CloseObjectivePanel()
	{
		m_ObjectivePanel.SetActive(false);
		AudioManager.Instance.PlayButtonClick();
	}

	IEnumerator SavePlayerInventoryRecur()
	{
		while(true)
		{
			yield return new WaitForSeconds(m_InventorySaveFrequency);

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
		}
	}
}
