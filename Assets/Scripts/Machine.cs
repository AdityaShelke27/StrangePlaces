using UnityEngine;

public class Machine : MonoBehaviour
{
    [SerializeField] int m_MachineID;
    [SerializeField] SurfaceNode m_InputType;
    [SerializeField] bool m_IsNodeInput;
    [SerializeField] ItemSlot m_Output;
    bool m_IsOn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsNodeInput() => m_IsNodeInput;
    public SurfaceNode GetInputType() => m_InputType;
    public int GetMachineID() => m_MachineID;
}
