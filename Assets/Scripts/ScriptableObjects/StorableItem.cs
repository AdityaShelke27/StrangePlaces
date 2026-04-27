using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StorableItem", menuName = "Scriptable Objects/StorableItem")]
public class StorableItem : Item
{
    public PlacementType PlacementType;
    public int StackableAmount;
    public GameObject WorldPrefab;
    public List<ResourceNode> PlacableNodes;
    public Vector2 Size;

    public List<ResourceNode> GetPlacableNodes() => PlacableNodes;
    public GameObject GetWorldPrefab() => WorldPrefab;
}
