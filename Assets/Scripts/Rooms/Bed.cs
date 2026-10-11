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

			PlayerPrefs.SetInt(Constant.PREF_DAY, PlayerPrefs.GetInt(Constant.PREF_DAY) + 1);
			SceneManager.LoadScene(Constant.SCENE_SURFACE);
		}));
	}
}
