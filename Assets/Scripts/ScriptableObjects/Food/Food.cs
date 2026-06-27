using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Scriptable Objects/Food")]
public class Food : StorableItem
{
	public uint hungerRestored;
	public string buffID;
}
