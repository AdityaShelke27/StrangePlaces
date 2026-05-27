using UnityEngine;

public class RoomSilhouette : MonoBehaviour
{
	RoomPlacement m_RoomPlacementScript;
	RoomStairPlacement m_DoorFacingDIrection;
	int m_GroundLevel;
	private void OnMouseUpAsButton()
	{
		Debug.Log("Clicking");
		m_RoomPlacementScript.ConstructRoomAtLocation(transform.position, m_DoorFacingDIrection, m_GroundLevel);
	}

	public void SetRoomPlacementScript(RoomPlacement _roomPlacement) => m_RoomPlacementScript = _roomPlacement;
	public void SetIsDoorFacingLeft(RoomStairPlacement _facingDirection) => m_DoorFacingDIrection = _facingDirection;
	public int GetGroundLevel() => m_GroundLevel;
	public int SetGroundLevel(int _groundLevel) => m_GroundLevel = _groundLevel;
}
