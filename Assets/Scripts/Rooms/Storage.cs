using UnityEngine;
using UnityEngine.EventSystems;

public class Storage : MonoBehaviour
{
	[SerializeField] private InventorySlot[] m_InventorySlots;
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		Debug.Log("Working");
	}
}
