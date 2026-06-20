using UnityEngine;

[CreateAssetMenu(fileName = "CraftObject", menuName = "Scriptable Objects/CraftObject")]
public class CraftObject : ScriptableObject
{
	public StorableItem CraftItem;
	public ResourceRequirement[] ResourceRequirements;
}
