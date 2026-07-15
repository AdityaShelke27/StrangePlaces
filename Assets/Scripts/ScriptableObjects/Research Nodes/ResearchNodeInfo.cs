using UnityEngine;

[CreateAssetMenu(fileName = "ResearchNode", menuName = "Scriptable Objects/ResearchNode")]
public class ResearchNodeInfo : ScriptableObject
{
	public int ID;
	public Sprite Icon;
	public string Name;
	public string Description;
	public int ResearchCost;
	public ResourceRequirement[] ResourceRequirements;
	public ResearchNodeInfo[] Prerequisites;
	public ResearchNodeInfo[] Unlocks;
}

