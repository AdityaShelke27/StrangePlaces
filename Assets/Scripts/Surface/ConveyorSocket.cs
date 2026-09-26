using UnityEngine;

public class ConveyorSocket : MonoBehaviour
{
	[SerializeField] bool m_IsInputSocket;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnMouseDown()
	{
		Debug.Log("Mouse Down");
		if(!ConveyorManager.Instance.GetIsInConveyorMode()) return;

		ConveyorManager.Instance.SetStartPos(transform.position);
	}

	private void OnMouseUp()
	{
		Debug.Log("Mouse Up");
		if (!ConveyorManager.Instance.GetIsInConveyorMode()) return;

		ConveyorManager.Instance.SetEndPos(transform.position);
	}
}
