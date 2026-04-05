using UnityEngine;

[CreateAssetMenu(fileName = "ResourceNode", menuName = "Scriptable Objects/ResourceNode")]
public class ResourceNode : Item
{
    public int MaxAmount;
    public SurfaceNode NodeType;
    public Resource ResourceYield;
}
