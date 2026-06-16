using UnityEngine;
using UnityEngine.EventSystems;

public class Room : MonoBehaviour
{
	[SerializeField] int m_RoomID;
	[SerializeField] int m_GroundLevel;
	[SerializeField] Transform m_PlayerPresentPoint;
	[SerializeField] E_RoomStairPlacement m_StairPlacement;
	[SerializeField] Collider2D[] m_ChildInteractables;
	BoxCollider2D m_Collider;
	private void Start()
	{
		m_Collider = GetComponent<BoxCollider2D>();
		SwitchToInteractableCollider(false);
	}
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		BunkerMovement.s_MoveHere?.Invoke(m_GroundLevel, m_PlayerPresentPoint.position.x, m_RoomID);
	}
	public void SwitchToInteractableCollider(bool _val)
	{
		m_Collider.enabled = !_val;

		foreach(Collider2D _col in m_ChildInteractables)
		{
			_col.enabled = _val;
		}
	}
	public void SetRoomFlipped(bool _val)
	{
		gameObject.GetComponent<SpriteRenderer>().flipX = _val;

		if(_val)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				Vector3 pos = transform.GetChild(i).localPosition;
				pos.x *= -1;
				transform.GetChild(i).localPosition = pos;
			}
		}
	}
	public Vector2 GetSize() => Constant.SIZE_ROOM;
	public E_RoomStairPlacement GetStairPlacement() => m_StairPlacement;
	public void SetGroundLevel(int _level) => m_GroundLevel = _level;
	public int GetGroundLevel() => m_GroundLevel;
	public int GetRoomID() => m_RoomID;
	public void SetStairPlacement(E_RoomStairPlacement _stairPlacement) => m_StairPlacement = _stairPlacement;
}
