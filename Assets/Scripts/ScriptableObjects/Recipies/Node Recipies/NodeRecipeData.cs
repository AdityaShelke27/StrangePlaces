using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Scriptable Objects/Recipe/NodeRecipeData")]
public class NodeRecipeData : RecipeData
{
    public ResourceNode Input;
    public List<ResourceAmount> Output;

    public float CraftTime; 
}
