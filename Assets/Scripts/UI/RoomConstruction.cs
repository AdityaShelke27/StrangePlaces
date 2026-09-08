using UnityEngine;
using UnityEngine.UI;

public class RoomConstruction : MonoBehaviour
{
	[SerializeField] GameObject RoomConstructionPanel;
	[SerializeField] GameObject[] m_NotResearchedPanel;
	bool m_IsRoomConstructionPanelActive = false;

	private void OnEnable()
	{
		ResearchNode.s_ResearchedAction += UnlockRoomResearched;
	}
	private void OnDisable()
	{
		ResearchNode.s_ResearchedAction -= UnlockRoomResearched;
	}
	void Start()
	{
		RoomConstructionPanel.SetActive(m_IsRoomConstructionPanelActive);

		for(int i = 0; i < m_NotResearchedPanel.Length; i++)
		{
			bool _isResearched = ItemDatabase.Instance.DoesItemIDExistInResearch(i.ToString());

			if (m_NotResearchedPanel[i] == null) continue;

			m_NotResearchedPanel[i].SetActive(!_isResearched);
			m_NotResearchedPanel[i].transform.parent.GetComponent<Button>().interactable = _isResearched;
		}
	}

	public void ToggleRoomConstructionPanel()
	{
		m_IsRoomConstructionPanelActive = !m_IsRoomConstructionPanelActive;
		RoomConstructionPanel.SetActive(m_IsRoomConstructionPanelActive);
	}
	void UnlockRoomResearched(int _id)
	{
		for (int i = 0; i < m_NotResearchedPanel.Length; i++)
		{
			bool _isResearched = ItemDatabase.Instance.DoesItemIDExistInResearch(i.ToString());

			if (m_NotResearchedPanel[i] == null) continue;

			m_NotResearchedPanel[i].SetActive(!_isResearched);
			m_NotResearchedPanel[i].transform.parent.GetComponent<Button>().interactable = _isResearched;
		}
	}
	public void GenerateConstructionRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_CONSTRUCTION_ROOM);
	}
	public void GenerateMechanicRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_MECHANIC_ROOM);
	}
	public void GenerateStorageRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_STORAGE_ROOM);
	}
	public void GenerateKitchen()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_KITCHEN_ROOM);
	}
	public void GenerateResearchRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_RESEARCH_ROOM);
	}
	public void GenerateRocketConstructionRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_ROCKETCONSTRUCTION_ROOM);
	}
}
