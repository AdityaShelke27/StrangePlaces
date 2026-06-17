using UnityEngine;
using UnityEngine.EventSystems;

public class Construction : MonoBehaviour
{
	[SerializeField] GameObject m_MachinePanelUI;

	private void Start()
	{
		ClosePanel();
	}
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		m_MachinePanelUI.SetActive(true);
	}

	public void ClosePanel() => m_MachinePanelUI.SetActive(false);
}
