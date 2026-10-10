using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject m_ContinueButton;

	private void Awake()
	{
		QualitySettings.vSyncCount = 0;
		// Dynamically match the screen's refresh rate (e.g., 60, 90, 120)
		Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
	}
	void Start()
    {
		m_ContinueButton.SetActive(PlayerPrefs.HasKey(Constant.PREF_AVAILABLE));
	}
	public void ContinueButton()
	{
		SceneManager.LoadScene(Constant.SCENE_BUNKER);
	}
	public void NewGameButton()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.SetInt(Constant.PREF_AVAILABLE, 1);
		SceneManager.LoadScene(Constant.SCENE_SURFACE);
	}
	public void ExitButton()
	{
		Application.Quit();
	}
}
