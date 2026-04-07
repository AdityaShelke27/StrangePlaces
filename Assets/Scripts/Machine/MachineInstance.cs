using System.Collections;
using UnityEngine;

public abstract class MachineInstance : WorldInstance
{
    /*[SerializeField] int m_MachineID;
    [SerializeField] SurfaceNode m_InputType;
    [SerializeField] bool m_IsNodeInput;
    [SerializeField] StorableItem m_Input;
    [SerializeField] ItemSlot m_Output;*/
    protected bool IsWorking;
    protected MachineState State = MachineState.Inactive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public abstract void StartMachine();

    public MachineState GetMachineState() => State;
    public abstract void SetMachineState(MachineState _state);
    /*IEnumerator MachineWork()
    {
        yield return null;
    }
    public bool IsNodeInput() => m_IsNodeInput;
    public SurfaceNode GetInputType() => m_InputType;
    public int GetMachineID() => m_MachineID;*/
}
