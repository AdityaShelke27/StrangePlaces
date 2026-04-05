using System;
using UnityEngine;

public class ResourceMachineInstance : MachineInstance
{
    private ResourceMachineIns m_MachineData;
    private ItemSlot m_Input;
    private ItemSlot m_Output;
    [SerializeField] private SurfaceNode m_InputType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Initialize(StorableItem data)
    {
        throw new NotImplementedException();
    }

    public SurfaceNode GetInputType() => m_InputType;

    public override void StartMachine()
    {
        throw new NotImplementedException();
    }

}
