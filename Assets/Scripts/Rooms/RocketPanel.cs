using UnityEngine;
using UnityEngine.EventSystems;

public class RocketPanel : MonoBehaviour
{
	public static RocketPanel Instance;
	[SerializeField] GameObject m_RCPanelUI;
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}
	private void Start()
	{
		ClosePanel();
	}
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		m_RCPanelUI.SetActive(true);
	}

	public void ClosePanel() => m_RCPanelUI.SetActive(false);
}
