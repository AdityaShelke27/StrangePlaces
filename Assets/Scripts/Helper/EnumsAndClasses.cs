using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum E_TerrainTypes
{
	Water,
	Sand,
	Grass,
	Rock
}
public enum E_SurfaceNode
{
	Ore_Node,
	Plant_Node,
	Gravitational_Anomaly_Node,
	Alien_Ruin_Node
}
public enum E_RoomStairPlacement
{
	No_Stairs,
	Right,
	Left
}
public enum E_Rooms
{
	Mechanic,
	Construction,
	Storage,
	Research,
	Kitchen,
	Rocket_Construction
}
public enum E_MachineState
{
	Inactive,
	Working,
	Halted
}
public enum E_PlacementType
{
	None,
	NodePlacement,
	FreePlacement
}
public enum E_Machine
{
	Miner,
	Harvester,
	Bio_Reactor,
	Artifact_Scanner,
	Smelter,
	Organic_Refinery,
	Gravity_Reactor,
	Research_Station
}
public enum E_ResearchStatus
{
	Locked,
	Researched,
	Available
}

public interface IActivate
{
	void Activate();
}

[Serializable]
public class ItemSlot
{
	public StorableItem item;
	public int amount;

	public ItemSlot()
	{
		item = null;
		amount = 0;
	}
	public ItemSlot(StorableItem _item, int _amount)
	{
		item = _item;
		amount = _amount;
	}
}
[Serializable]
public class ResourceRequirement
{
	public Item item;
	public int amount;
}
[Serializable]
public static class PlayerData
{
	public static ItemSlot[] itemSlot = new ItemSlot[5];
	public static int hunger;
	public static int electricity;
	public static int researchPoints;
	public static bool isDataSaved = false;

	public static void LoadData()
	{
		if (!PlayerPrefs.HasKey(Constant.PREF_SAVE_PLAYERDATA)) return;

		Save_PlayerData _playerData = JsonUtility.FromJson<Save_PlayerData>(PlayerPrefs.GetString(Constant.PREF_SAVE_PLAYERDATA));
		hunger = _playerData.hunger;
		electricity = _playerData.electricity;
		researchPoints = _playerData.researchPoints;
		Save_ItemSlotArray[] _items = _playerData.itemSlotArray;
		for (int i = 0; i < _items.Length; i++)
		{
			itemSlot[i] = string.IsNullOrEmpty(_items[i].id) ? new() : new(ItemDatabase.Instance.GetItemByID(_items[i].id) as StorableItem, _items[i].amount);
		}
	}
	public static void SaveData()
	{
		Save_ItemSlotArray[] _saveItems = new Save_ItemSlotArray[itemSlot.Length];
		for (int i = 0; i < itemSlot.Length; i++)
		{
			StorableItem _item = itemSlot[i].item;
			_saveItems[i] = _item != null ? new(_item.itemID, itemSlot[i].amount) : new("", 0);
		}

		PlayerPrefs.SetString(Constant.PREF_SAVE_PLAYERDATA, JsonUtility.ToJson(new Save_PlayerData(_saveItems, hunger, electricity, researchPoints)));
	}
}
[Serializable]
public class Save_ItemSlotArray
{
	public string id;
	public int amount;
	public Save_ItemSlotArray(string _id, int _amount)
	{
		id = _id;
		amount = _amount;
	}
}
[Serializable]
public class Save_PlayerData
{
	public Save_ItemSlotArray[] itemSlotArray;
	public int hunger;
	public int electricity;
	public int researchPoints;

	public Save_PlayerData(Save_ItemSlotArray[] _itemSlotArray, int _hunger, int _electricity, int _researchPoints)
	{
		itemSlotArray = _itemSlotArray;
		hunger = _hunger;
		electricity = _electricity;
		researchPoints = _researchPoints;
	}
}
[Serializable]
public class Save_Room
{
	public int RoomID;
	public int GroundLevel;
	public Vector3 Pos;
	public E_RoomStairPlacement DoorDir;

	public Save_Room(int _roomID, int _groundLevel, Vector3 _pos, E_RoomStairPlacement _doorFacingDirection)
	{
		RoomID = _roomID;
		GroundLevel = _groundLevel;
		Pos = _pos;
		DoorDir = _doorFacingDirection;
	}
}
[Serializable]
public class Save_RoomData
{
	public List<Save_Room> Rooms;
	public Save_RoomData()
	{
		Rooms = new();
	}
	public Save_RoomData(Save_Room[] _rooms) 
	{ 
		Rooms = _rooms.ToList(); 
	}
	public Save_RoomData(List<Save_Room> _rooms)
	{
		Rooms = _rooms;
	}
	public void AddRoom(Save_Room _room)
	{
		Rooms.Add(_room);
	}
	public static void SaveData(Save_RoomData _roomData)
	{
		PlayerPrefs.SetString(Constant.PREF_ROOMSUNLOCKED, JsonUtility.ToJson(_roomData));
	}
	public static Save_RoomData LoadData()
	{
		string _dataStr = PlayerPrefs.GetString(Constant.PREF_ROOMSUNLOCKED, "");

		if(string.IsNullOrEmpty(_dataStr)) return null;

		Save_RoomData _data = JsonUtility.FromJson<Save_RoomData>(_dataStr);

		return _data;
	}
	public static void AppendNewRoom(Save_Room _room)
	{
		if(_room == null) return;

		Save_RoomData _roomData = LoadData();
		_roomData ??= new();
		_roomData.AddRoom(_room);
		SaveData(_roomData);
	}
}
