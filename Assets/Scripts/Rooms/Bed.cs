using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour
{
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		SceneManager.LoadScene(Constant.SCENE_SURFACE);
	}
}
