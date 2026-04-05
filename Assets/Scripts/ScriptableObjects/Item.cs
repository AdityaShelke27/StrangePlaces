using UnityEngine;

[CreateAssetMenu(fileName = "StorableItem", menuName = "Scriptable Objects/StorableItem")]
public class Item : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite itemImage;
    public string itemDescription;
}
