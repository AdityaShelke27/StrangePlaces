using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Scriptable Objects/Recipe/ResourceRecipeData")]
public class ResourceRecipeData : RecipeData
{
    public List<ResourceAmount> Input;
    public List<ResourceAmount> Output;

    public float CraftTime; 
}

[System.Serializable]
public struct ResourceAmount
{
    public Resource Resource;
    public int amount;
}
