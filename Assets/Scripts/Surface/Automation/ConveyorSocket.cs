using UnityEngine;

public class ConveyorSocket : MonoBehaviour
{
	[SerializeField] MachineInstance m_MachineScript;
	[SerializeField] bool m_IsInputSocket;
	[SerializeField] bool m_IsBunkerSocket = false;
	bool m_IsSocketConnected = false;

	public bool IsInputSocket => m_IsInputSocket;
	public bool IsBunkerSocket => m_IsBunkerSocket;
	private void OnMouseDown()
	{
		Debug.Log("Mouse Down");
		if(!ConveyorManager.Instance.GetIsInConveyorMode() || m_IsInputSocket) return;

		ConveyorManager.Instance.BeginConnection(this);
	}

	public MachineInstance GetMachineScript() => m_MachineScript;
	public bool GetSocketConnected() => m_IsSocketConnected;
	public void SetSocketConnected(bool _val) => m_IsSocketConnected = _val;
}
