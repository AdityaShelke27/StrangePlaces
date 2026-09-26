using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceMachine", menuName = "Scriptable Objects/Machine/ResourceMachine")]
public class ResourceMachine : Machine
{
	public int InputSlots;
	public Vector2 ConveyorInputPos;
	public Vector2 ConveyorOutputPos;
	public ResourceRecipeData[] RecipeData;
}
