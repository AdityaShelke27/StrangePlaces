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
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, E_Rooms.Construction);
	}
	public void GenerateStorageRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, E_Rooms.Storage);
	}
	public void GenerateKitchen()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, E_Rooms.Kitchen);
	}
	public void GenerateResearchRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, E_Rooms.Research);
	}
	public void GenerateRocketConstructionRoom()
	{
		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.ROOM_SIZE, E_Rooms.Rocket_Construction);
	}
}
