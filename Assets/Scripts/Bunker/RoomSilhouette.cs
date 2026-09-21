using System;
using UnityEngine;

public class RoomSilhouette : MonoBehaviour
{
	RoomPlacement m_RoomPlacementScript;
	E_RoomStairPlacement m_DoorFacingDirection;
	int m_RoomID;
	int m_GroundLevel;

	Action m_Action;
	private void OnMouseUpAsButton()
	{
		m_Action?.Invoke();
		m_RoomPlacementScript.ConstructRoomAtLocation(transform.position, m_DoorFacingDirection, m_GroundLevel, m_RoomID);
	}
	public void SetInfo(RoomPlacement _roomPlacement, E_RoomStairPlacement _facingDirection, int _groundLevel, int _roomID, Action _action)
	{
		m_RoomPlacementScript = _roomPlacement;
		m_DoorFacingDirection = _facingDirection;
		m_GroundLevel = _groundLevel;
		m_RoomID = _roomID;

		m_Action = _action;
	}
	public int GetGroundLevel() => m_GroundLevel;
}
