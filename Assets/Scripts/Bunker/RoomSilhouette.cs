using UnityEngine;

public class RoomSilhouette : MonoBehaviour
{
	RoomPlacement m_RoomPlacementScript;
	E_RoomStairPlacement m_DoorFacingDIrection;
	int m_RoomID;
	int m_GroundLevel;
	private void OnMouseUpAsButton()
	{
		m_RoomPlacementScript.ConstructRoomAtLocation(transform.position, m_DoorFacingDIrection, m_GroundLevel, m_RoomID);
	}
	public void SetInfo(RoomPlacement _roomPlacement, E_RoomStairPlacement _facingDirection, int _groundLevel, int _roomID)
	{
		m_RoomPlacementScript = _roomPlacement;
		m_DoorFacingDIrection = _facingDirection;
		m_GroundLevel = _groundLevel;
		m_RoomID = _roomID;
	}
	public int GetGroundLevel() => m_GroundLevel;
}
