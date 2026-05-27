using UnityEngine;
using UnityEngine.EventSystems;

public class Room : MonoBehaviour
{
	[SerializeField] int m_GroundLevel;
	[SerializeField] RoomStairPlacement m_StairPlacement;
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		float _Xpos = Camera.main.ScreenToWorldPoint(InputManager.GetTouchPosition()).x;
		BunkerMovement.s_MoveHere?.Invoke(m_GroundLevel, _Xpos);
	}

	public Vector2 GetSize() => Constant.ROOM_SIZE;
	public RoomStairPlacement GetStairPlacement() => m_StairPlacement;
	public void SetGroundLevel(int _level) => m_GroundLevel = _level;
	public int GetGroundLevel() => m_GroundLevel;
	public void SetStairPlacement(RoomStairPlacement _stairPlacement) => m_StairPlacement = _stairPlacement;
}
