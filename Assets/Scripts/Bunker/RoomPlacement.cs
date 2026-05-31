using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacement : MonoBehaviour
{
	public static Action<Vector2> s_GenerateAreas;

	[SerializeField] float m_RoomSpacing;
	[SerializeField] float m_StairPointOffset;
	[SerializeField] Sprite m_BlankSprite;
	[SerializeField] Sprite m_StairSprite;
	[SerializeField] Transform m_RoomListParent;
	[SerializeField] Transform m_StairsParent;
	[SerializeField] Transform m_GroundLevelPointsParent;
	[SerializeField] GameObject m_RoomPrefab;
	[SerializeField] int m_BuiltGroundLevel;
	List<GameObject> m_RoomSilhouletteList = new();

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
				RoomStairPlacement _PlacementDirection;
				int _groundLevel = 0;
				if (j == 0 && _currentRoom.GetStairPlacement() == RoomStairPlacement.Left)
				{
					_placement = _targetPoses[j] + Constant.STAIR_SIZE.x * Vector2.left;
					_PlacementDirection = RoomStairPlacement.Right;
					_groundLevel = _currentRoom.GetGroundLevel();
				}
				else if(j == 1 && _currentRoom.GetStairPlacement() == RoomStairPlacement.Right)
				{
					_placement = _targetPoses[j] + Constant.STAIR_SIZE.x * Vector2.right;
					_PlacementDirection = RoomStairPlacement.Left;
					_groundLevel = _currentRoom.GetGroundLevel();
				}
				else if(j == 0 || j == 1)
				{
					continue;
				}
				else
				{
					_placement = _targetPoses[j];

					_PlacementDirection = _currentRoom.GetStairPlacement();

					if(j == 2)
					{
						_groundLevel = _currentRoom.GetGroundLevel() - 1;
					}
					else if(j == 3)
					{
						_groundLevel = _currentRoom.GetGroundLevel() + 1;
					}

					if (_groundLevel < 1) continue;
				}

				if (_SearchedPoses.Contains(_placement)) continue;

				_SearchedPoses.Add(_placement);

				Collider2D _col = Physics2D.OverlapBox(_placement, _roomSize, 0);
				Debug.Log($"Silhouette {j}");
				if (_col == null)
				{
					GameObject _obj = new("Placement_Effect", typeof(SpriteRenderer), typeof(RoomSilhouette), typeof(BoxCollider2D));
					_obj.GetComponent<BoxCollider2D>().size = _obj.transform.localScale;
					RoomSilhouette _script = _obj.GetComponent<RoomSilhouette>();
					SpriteRenderer _renderer = _obj.GetComponent<SpriteRenderer>();
					_renderer.sprite = m_BlankSprite;
					_renderer.color = Color.green;

					_obj.transform.position = _placement;
					_obj.transform.localScale = new(_roomSize.x, _roomSize.y, 1);

					m_RoomSilhouletteList.Add(_obj);
					_script.SetRoomPlacementScript(this);
					_script.SetIsDoorFacingLeft(_PlacementDirection);
					_script.SetGroundLevel(_groundLevel);
				}
			}
		}
	}

	public void ConstructRoomAtLocation(Vector3 _pos, RoomStairPlacement m_DoorFacingDirection, int _groundLevel)
	{
		GameObject _room = Instantiate(m_RoomPrefab, _pos, Quaternion.identity);
		_room.transform.parent = m_RoomListParent;
		Room _roomScript = _room.GetComponent<Room>();

		bool _facingLeft = false;
		switch(m_DoorFacingDirection)
		{
			case RoomStairPlacement.Left:
				_facingLeft = true; 
				break;
			case RoomStairPlacement.Right:
				_facingLeft = false;
				break;
			case RoomStairPlacement.No_Stairs: 
				_facingLeft = false; 
				break;
		}
		_room.GetComponent<SpriteRenderer>().flipX = !_facingLeft;
		_roomScript.SetStairPlacement(m_DoorFacingDirection);
		_roomScript.SetGroundLevel(_groundLevel);

		foreach (GameObject _obj in m_RoomSilhouletteList)
		{
			Destroy(_obj);
		}

		m_RoomSilhouletteList.Clear();

		if (_groundLevel > m_BuiltGroundLevel) ConstructNewGroundLevel();
	}

	void ConstructNewGroundLevel()
	{
		m_BuiltGroundLevel++;

		GameObject _stairsObj = new("Stairs", typeof(SpriteRenderer));
		_stairsObj.GetComponent<SpriteRenderer>().sprite = m_StairSprite;
		_stairsObj.transform.parent = m_StairsParent;
		_stairsObj.transform.localPosition = Constant.STAIR_SIZE.y * (m_BuiltGroundLevel - 1) * Vector3.down;

		GameObject _groundPoint = new("Point");
		_groundPoint.transform.parent = m_GroundLevelPointsParent;
		_groundPoint.transform.localPosition = (Constant.STAIR_SIZE.y * m_BuiltGroundLevel + m_StairPointOffset) * Vector3.down;
	}
}
