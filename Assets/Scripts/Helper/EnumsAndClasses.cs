using System;
using static UnityEditor.Progress;

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
public class Save_ItemSlot
{
	public string id;
	public int amount;
	public Save_ItemSlot(string _id, int _amount)
	{
		id = _id;
		amount = _amount;
	}
}

