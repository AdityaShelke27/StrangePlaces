using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject m_ContinueButton;
	[SerializeField] private GameObject m_TutorialPanel;

	private void Awake()
	{
		QualitySettings.vSyncCount = 0;
		// Dynamically match the screen's refresh rate (e.g., 60, 90, 120)
		Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
	}
	void Start()
    {
		m_TutorialPanel.SetActive(false);
		m_ContinueButton.SetActive(PlayerPrefs.HasKey(Constant.PREF_SAVEAVAILABLE));
	}
	public void ContinueButton()
	{
		AudioManager.Instance.PlayButtonClick();
		SceneManager.LoadScene(Constant.SCENE_BUNKER);
	}
	public void NewGameButton()
	{
		AudioManager.Instance.PlayButtonClick();
		m_TutorialPanel.SetActive(true);
	}
	void PlayNewGame()
	{
		AudioManager.Instance.PlayButtonClick();

		PlayerPrefs.SetInt(Constant.PREF_SAVEAVAILABLE, 1);
		PlayerPrefs.SetInt(Constant.PREF_DAY, 1);
		SceneManager.LoadScene(Constant.SCENE_SURFACE);
	}
	public void Tutorial_YesButton()
	{
		AudioManager.Instance.PlayButtonClick();

		PlayerPrefs.DeleteAll();
		PlayerPrefs.SetInt(Constant.PREF_TUTORIAL, 1);
		PlayNewGame();
	}
	public void Tutorial_NoButton()
	{
		AudioManager.Instance.PlayButtonClick();

		PlayerPrefs.DeleteAll();
		PlayerPrefs.SetInt(Constant.PREF_TUTORIAL, 0);
		PlayNewGame();
	}
	public void Tutorial_CloseButton()
	{
		AudioManager.Instance.PlayButtonClick();
		m_TutorialPanel.SetActive(false);
	}
	public void ExitButton()
	{
		AudioManager.Instance.PlayButtonClick();
		Application.Quit();
	}
}
