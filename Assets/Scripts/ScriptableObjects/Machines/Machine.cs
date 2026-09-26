using UnityEngine;

[CreateAssetMenu(fileName = "Machine", menuName = "Scriptable Objects/Machine")]
public class Machine : StorableItem
{
	public float TimeToProduce = 10f;
	public float MachineHaltCheck = 1f;
	public int ElectricityConsumption = 1;
	public int OutputSlots;
}
