using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public enum SurfaceNode
{
    Ore_Node,
    Plant_Node,
    Gravitational_Anomaly_Node,
    Alien_Ruin_Node
}
public enum SurfaceNodeAmount
{
    Small,
    Medium,
    Large,
    Very_Large
}
public enum ResourceType
{
    Ore,
    Plant,
    Alien_Artifact,
    Metal,
    Biomass,
    Electricity
}
public enum StorableItemEnum
{
    Alien_Artifact,
    Artifact_Scanner,
    Axe,
    Biomass,
    BioProcessor,
    BioReactor,
    Gravity_Reactor,
    Harvester,
    Metal,
    Miner,
    Ore,
    Pickaxe,
    Plant,
    Research_Station,
    Smelter
}

public enum MachineState
{
    Inactive,
    Working,
    Halted
}
public enum PlacementType
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
public abstract class MachineIns : Item
{
    protected GameObject MachinePrefab;
    protected float PowerConsumption;

    public MachineIns(GameObject _MachinePrefab)
    {
        MachinePrefab = _MachinePrefab;
    }

    public GameObject GetMachinePrefab() => MachinePrefab;
}
class NodeMachineIns : MachineIns
{
    protected ResourceNodeInstance input;
    protected ResourceIns output;

    public NodeMachineIns(GameObject _MachinePrefab) : base(_MachinePrefab)
    {
        
    }
}
class ResourceMachineIns : MachineIns
{
    protected ResourceIns input;
    protected ResourceIns output;

    public ResourceMachineIns(GameObject _MachinePrefab) : base(_MachinePrefab)
    {

    }
}
class ResourceIns : Item
{
    
}
class Tool0 : Item
{
    private float maxDurability;
    private float durability;
    private ResourceNodeInstance minableNode;
}