using System;
using System.Collections;
using UnityEngine;

public class BunkerMovement : MonoBehaviour
{
	public static Action<int, float, int> s_MoveHere;

	[SerializeField] Transform m_PointsParent;
	[SerializeField] Transform m_MainCam;
	[SerializeField] Transform m_RoomListParent;
	[SerializeField] int m_CurrentRoomID;
	[SerializeField] float m_CamMoveSpeed;
	[SerializeField] float m_CameraYOffset;
	[SerializeField] int m_CurrentGroundLevel = 0;
	[SerializeField] float m_Speed = 10;

	bool m_IsMoving = false;

	private void OnEnable()
	{
		s_MoveHere += MoveToPoint;
	}
	private void OnDisable()
	{
		s_MoveHere -= MoveToPoint;
	}
	private void Update()
	{
		Vector3 pos = m_MainCam.transform.position;
		pos.y = Mathf.Lerp(pos.y, transform.position.y + m_CameraYOffset, Time.deltaTime * m_CamMoveSpeed);
		pos.z = -10;
		m_MainCam.transform.position = pos;
	}
	void MoveToPoint(int _groundLevel, float _pointX, int _roomID)
	{
		if (m_IsMoving) return;

		StartCoroutine(Cor_MoveToPoint(_groundLevel, _pointX, _roomID));
	}
	IEnumerator Cor_MoveToPoint(int _groundLevel, float _pointX, int _roomID)
	{
		m_IsMoving = true;

		Vector2 _movePos, _dir;
		if (_groundLevel != m_CurrentGroundLevel)
		{
			_movePos = m_PointsParent.GetChild(m_CurrentGroundLevel).position;
			_dir = (_movePos - (Vector2)transform.position).normalized;
			while (Vector2.Distance(transform.position, _movePos) > 0.01f)
			{
				transform.Translate(m_Speed * Time.deltaTime * _dir);
				yield return null;
			}

			_movePos = m_PointsParent.GetChild(_groundLevel).position;
			_dir = (_movePos - (Vector2)transform.position).normalized;
			while (Vector2.Distance(transform.position, _movePos) > 0.01f)
			{
				transform.Translate(m_Speed * Time.deltaTime * _dir);
				yield return null;
			}
		}

		_movePos = new Vector2(_pointX, m_PointsParent.GetChild(_groundLevel).position.y);
		_dir = (_movePos - (Vector2)transform.position).normalized;
		while (Vector2.Distance(transform.position, _movePos) > 0.01f)
		{
			transform.Translate(m_Speed * Time.deltaTime * _dir);
			yield return null;
		}

		m_CurrentGroundLevel = _groundLevel;

		m_IsMoving = false;

		for(int i = 0; i < m_RoomListParent.childCount; i++)
		{
			Room _room = m_RoomListParent.GetChild(i).GetComponent<Room>();

			_room.GetComponent<BoxCollider2D>().enabled = _room.GetRoomID() != _roomID;
		}
		m_CurrentRoomID = _roomID;
	}
}
