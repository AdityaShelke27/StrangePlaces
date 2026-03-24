using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "Scriptable Objects/Tool")]
public class Tool : StorableItem
{
    public int maxDurability;
    public float miningSpeed;
}
