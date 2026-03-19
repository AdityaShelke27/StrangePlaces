using UnityEngine;

public class Machine : Item
{
    [SerializeField] int m_MachineID;
    [SerializeField] SurfaceNode m_InputType;
    [SerializeField] bool m_IsNodeInput;
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
