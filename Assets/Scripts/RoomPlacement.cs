using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacement : MonoBehaviour
{
	public static Action<Vector2> s_GenerateAreas;

	[SerializeField] float m_RoomSpacing;
	[SerializeField] Sprite m_BlankSprite;
	[SerializeField] Transform m_RoomListParent;

	private void OnEnable()
	{
		s_GenerateAreas += GenerateAvailableAreas;
	}
	private void OnDisable()
	{
		s_GenerateAreas -= GenerateAvailableAreas;
	}

	void GenerateAvailableAreas(Vector2 _roomSize)
	{
		Debug.Log("Generate Area");

		List<Vector2> _SearchedPoses = new();

		for(int i = 0; i < m_RoomListParent.childCount; i++)
		{
			Room _currentRoom = m_RoomListParent.GetChild(i).GetComponent<Room>();
			Vector2 _pos = m_RoomListParent.GetChild(i).position;
			Vector2 _size = _currentRoom.GetSize();

			Debug.Log($"Room {_currentRoom.GetStairPlacement()}");

			Vector2[] _targetPoses =
			{
				_pos + Vector2.left * (((_size.x + _roomSize.x) / 2) + m_RoomSpacing),
				_pos + Vector2.right * (((_size.x + _roomSize.x) / 2) + m_RoomSpacing),
				_pos + Vector2.up * (((_size.y + _roomSize.y) / 2) + m_RoomSpacing),
				_pos + Vector2.down * (((_size.y + _roomSize.y) / 2) + m_RoomSpacing),
			};

			for(int j = 0; j < _targetPoses.Length; j++)
			{
				Vector2 _placement;
				if (j == 0 && _currentRoom.GetStairPlacement() == RoomStairPlacement.Left)
				{
					_placement = _targetPoses[j] + Constant.STAIR_SIZE.x * Vector2.left;
					
				}
				else if(j == 1 && _currentRoom.GetStairPlacement() == RoomStairPlacement.Right)
				{
					_placement = _targetPoses[j] + Constant.STAIR_SIZE.x * Vector2.right;
				}
				else
				{
					_placement = _targetPoses[j];
				}

				if (_SearchedPoses.Contains(_placement)) continue;

				_SearchedPoses.Add(_placement);

				Collider2D _col = Physics2D.OverlapBox(_placement, _roomSize, 0);

				if (_col == null)
				{
					GameObject _obj = new("Placement_Effect", typeof(SpriteRenderer));
					SpriteRenderer _renderer = _obj.GetComponent<SpriteRenderer>();
					_renderer.sprite = m_BlankSprite;
					_renderer.color = Color.green;

					_obj.transform.position = _placement;
					_obj.transform.localScale = new(_roomSize.x, _roomSize.y, 1);
				}
			}
		}
	}
}
