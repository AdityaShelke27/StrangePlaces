using UnityEngine;

[CreateAssetMenu(fileName = "ResourceMachine", menuName = "Scriptable Objects/Machine/ResourceMachine")]
public class ResourceMachine : Machine
{
    public int InputSlots;
    public ResourceRecipeData[] RecipeData;
}
