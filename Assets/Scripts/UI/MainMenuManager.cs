using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject m_ContinueButton;
    void Start()
    {
        m_ContinueButton.SetActive(false);
    }
	public void ContinueButton()
	{

	}
	public void NewGameButton()
	{
		SceneManager.LoadScene(Constant.SCENE_SURFACE);
	}
	public void ExitButton()
	{
		Application.Quit();
	}
}
