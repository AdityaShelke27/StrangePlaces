using UnityEngine;

public class BunkerManager : MonoBehaviour
{
	public static BunkerManager Instance;

	[SerializeField] private GameObject m_TutorialManager;
	[SerializeField] private GameObject m_ObjectivePanel;
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
		CloseObjectivePanel();

		TutorialManager.s_Trigger?.Invoke("0");
	}

	public void ShowObjectivePanel()
	{
		m_ObjectivePanel.SetActive(true);
	}

	public void CloseObjectivePanel()
	{
		m_ObjectivePanel.SetActive(false);
	}
}
