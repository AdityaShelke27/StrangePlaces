using UnityEngine;

public class RoomConstruction : MonoBehaviour
{
	[SerializeField] GameObject RoomConstructionPanel;
	bool m_IsRoomConstructionPanelActive = false;
	void Start()
	{
		RoomConstructionPanel.SetActive(m_IsRoomConstructionPanelActive);
	}

	public void ToggleRoomConstructionPanel()
	{
		m_IsRoomConstructionPanelActive = !m_IsRoomConstructionPanelActive;

		RoomConstructionPanel.SetActive(m_IsRoomConstructionPanelActive);
	}

	public void GenerateConstructionRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, Constant.ID_CONSTRUCTION_ROOM);
	}
	public void GenerateMechanicRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, Constant.ID_MECHANIC_ROOM);
	}
	public void GenerateStorageRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, Constant.ID_STORAGE_ROOM);
	}
	public void GenerateKitchen()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, Constant.ID_KITCHEN_ROOM);
	}
	public void GenerateResearchRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, Constant.ID_RESEARCH_ROOM);
	}
	public void GenerateRocketConstructionRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, Constant.ID_ROCKETCONSTRUCTION_ROOM);
	}
}
