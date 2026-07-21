using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Research : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] GameObject m_MainResearchPanel;
	[SerializeField] GameObject m_RocketResearchPanel;
	[SerializeField] GameObject m_ResearchCanvas;
	private void Start()
	{
		m_ResearchCanvas.SetActive(false);
	}
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		m_ResearchCanvas.SetActive(true);
		SelectMainResearch();
	}

	public void SelectMainResearch()
	{
		m_MainResearchPanel.SetActive(true);
		m_RocketResearchPanel.SetActive(false);
	}
	public void SelectRocketResearch()
	{
		m_MainResearchPanel.SetActive(false);
		m_RocketResearchPanel.SetActive(true);
	}

	public void ClosePanel() => m_ResearchCanvas.SetActive(false);
}
