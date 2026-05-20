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

	public Vector2 GetSize() => new(7.68f, 3.84f);
	public RoomStairPlacement GetStairPlacement() => m_StairPlacement;
}
