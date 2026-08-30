using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacement : MonoBehaviour
{
	public static Action<Vector2, int> s_GenerateAreas;

	[SerializeField] float m_RoomSpacing;
	[SerializeField] float m_StairPointOffset;
	[SerializeField] Sprite m_BlankSprite;
	[SerializeField] Sprite m_StairSprite;
	[SerializeField] Transform m_RoomListParent;
	[SerializeField] Transform m_StairsParent;
	[SerializeField] Transform m_GroundLevelPointsParent;
	[SerializeField] int m_BuiltGroundLevel;
	[SerializeField] GameObject[] m_Rooms;
	[SerializeField] GameObject[] m_RoomConstructionButtons;
	List<GameObject> m_RoomSilhouletteList = new();
	private void OnEnable()
	{
		s_GenerateAreas += GenerateAvailableAreas;
	}
	private void OnDisable()
	{
		s_GenerateAreas -= GenerateAvailableAreas;
	}

	private void Start()
	{
		Save_RoomData _roomData = Save_RoomData.LoadData();
		if (_roomData == null) return;

		foreach(Save_Room _room in _roomData.Rooms)
		{
			ConstructRoomDirectly(_room.Pos, _room.DoorDir, _room.GroundLevel, _room.RoomID);
		}
	}

	void GenerateAvailableAreas(Vector2 _roomSize, int _roomID)
	{
		List<Vector2> _SearchedPoses = new();

		for (int i = 0; i < m_RoomListParent.childCount; i++)
		{
			Room _room = m_RoomListParent.GetChild(i).GetComponent<Room>();

			_room.SwitchToInteractableCollider(false);
		}

		for (int i = 0; i < m_RoomListParent.childCount; i++)
		{
			Room _currentRoom = m_RoomListParent.GetChild(i).GetComponent<Room>();
			Vector2 _pos = m_RoomListParent.GetChild(i).position;
			Vector2 _size = _currentRoom.GetSize();

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
				E_RoomStairPlacement _PlacementDirection;
				int _groundLevel = 0;
				if (j == 0 && _currentRoom.GetStairPlacement() == E_RoomStairPlacement.Left)
				{
					_placement = _targetPoses[j] + Constant.SIZE_STAIR.x * Vector2.left;
					_PlacementDirection = E_RoomStairPlacement.Right;
					_groundLevel = _currentRoom.GetGroundLevel();
				}
				else if(j == 1 && _currentRoom.GetStairPlacement() == E_RoomStairPlacement.Right)
				{
					_placement = _targetPoses[j] + Constant.SIZE_STAIR.x * Vector2.right;
					_PlacementDirection = E_RoomStairPlacement.Left;
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

					if(j == 2) _groundLevel = _currentRoom.GetGroundLevel() - 1;
					else if(j == 3) _groundLevel = _currentRoom.GetGroundLevel() + 1;

					if (_groundLevel < 1) continue;
				}

				if (_SearchedPoses.Contains(_placement)) continue;

				_SearchedPoses.Add(_placement);

				Collider2D _col = Physics2D.OverlapBox(_placement, _roomSize, 0);
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
					_script.SetInfo(this, _PlacementDirection, _groundLevel, _roomID);
				}
			}
		}

		int _currentRoomID = BunkerMovement.instance.GetCurrentRoomID();
		for (int i = 0; i < m_RoomListParent.childCount; i++)
		{
			Room _room = m_RoomListParent.GetChild(i).GetComponent<Room>();
			_room.SwitchToInteractableCollider(_room.GetRoomID() == _currentRoomID);
		}
	}

	public void ConstructRoomAtLocation(Vector3 _pos, E_RoomStairPlacement _doorFacingDirection, int _groundLevel, int _roomID)
	{
		GameObject _room = Instantiate(m_Rooms[_roomID], _pos, Quaternion.identity);
		_room.transform.parent = m_RoomListParent;
		Room _roomScript = _room.GetComponent<Room>();

		bool _facingLeft = false;
		switch(_doorFacingDirection)
		{
			case E_RoomStairPlacement.Left:
				_facingLeft = true; 
				break;
			case E_RoomStairPlacement.Right:
				_facingLeft = false;
				break;
			case E_RoomStairPlacement.No_Stairs: 
				_facingLeft = false; 
				break;
		}
		_roomScript.SetRoomFlipped(!_facingLeft);
		_roomScript.SetStairPlacement(_doorFacingDirection);
		_roomScript.SetGroundLevel(_groundLevel);

		foreach (GameObject _obj in m_RoomSilhouletteList)
		{
			Destroy(_obj);
		}

		m_RoomSilhouletteList.Clear();
		if (m_RoomConstructionButtons[_roomID] != null) Destroy(m_RoomConstructionButtons[_roomID]);

		if (_groundLevel > m_BuiltGroundLevel) ConstructNewGroundLevel();

		Save_Room _roomData = new(_roomID, _groundLevel, _pos, _doorFacingDirection);
		Save_RoomData.AppendNewRoom(_roomData);
	}

	void ConstructNewGroundLevel()
	{
		m_BuiltGroundLevel++;

		GameObject _stairsObj = new("Stairs", typeof(SpriteRenderer));
		_stairsObj.GetComponent<SpriteRenderer>().sprite = m_StairSprite;
		_stairsObj.transform.parent = m_StairsParent;
		_stairsObj.transform.localPosition = (Constant.SIZE_STAIR.y + m_RoomSpacing) * (m_BuiltGroundLevel - 1) * Vector3.down;

		GameObject _groundPoint = new("Point");
		_groundPoint.transform.parent = m_GroundLevelPointsParent;
		_groundPoint.transform.localPosition = ((Constant.SIZE_STAIR.y + m_RoomSpacing) * m_BuiltGroundLevel + m_StairPointOffset) * Vector3.down;
	}

	public void ConstructRoomDirectly(Vector3 _pos, E_RoomStairPlacement _doorFacingDirection, int _groundLevel, int _roomID)
	{
		GameObject _room = Instantiate(m_Rooms[_roomID], _pos, Quaternion.identity);
		_room.transform.parent = m_RoomListParent;
		Room _roomScript = _room.GetComponent<Room>();

		bool _facingLeft = false;
		switch (_doorFacingDirection)
		{
			case E_RoomStairPlacement.Left:
				_facingLeft = true;
				break;
			case E_RoomStairPlacement.Right:
				_facingLeft = false;
				break;
			case E_RoomStairPlacement.No_Stairs:
				_facingLeft = false;
				break;
		}
		_roomScript.SetRoomFlipped(!_facingLeft);
		_roomScript.SetStairPlacement(_doorFacingDirection);
		_roomScript.SetGroundLevel(_groundLevel);

		if (m_RoomConstructionButtons[_roomID] != null) Destroy(m_RoomConstructionButtons[_roomID]);

		if (_groundLevel > m_BuiltGroundLevel) ConstructNewGroundLevel();
	}
}
