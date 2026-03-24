using UnityEngine;

[CreateAssetMenu(fileName = "ResourceNode", menuName = "Scriptable Objects/ResourceNode")]
public class ResourceNode : Item
{
    public Item outputResource;
    public int resourceAmount;
}
