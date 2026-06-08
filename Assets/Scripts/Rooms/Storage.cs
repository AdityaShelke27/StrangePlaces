using UnityEngine;
using UnityEngine.EventSystems;

public class Storage : MonoBehaviour
{
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		Debug.Log("Working");
	}
}
