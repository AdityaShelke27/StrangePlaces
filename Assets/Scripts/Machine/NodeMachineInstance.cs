using System;
using System.Collections;
using UnityEngine;

public class NodeMachineInstance : MachineInstance
{
    [SerializeField] private NodeMachine m_MachineData;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private ResourceNodeInstance m_Input;
    Coroutine m_MachineWorkingCoroutine;
    Coroutine m_MachineHaultedCoroutine;
    private ItemSlot m_Output;

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
        m_MachineData = data as NodeMachine;
        m_SpriteRenderer.sprite = m_MachineData.itemImage;
        GetComponent<BoxCollider2D>().size = m_MachineData.Size;
    }
    public void SetInputNode(ResourceNodeInstance _input)
    {
        m_Input = _input;
    }

    public override void StartMachine()
    {
        if(!m_Input)
        {
            Debug.LogWarning("Input Empty");
            SetMachineState(MachineState.Halted);
            return;
        }
        SetMachineState(MachineState.Working);
    }

    IEnumerator MachineWork()
    {
        State = MachineState.Working;

        while(m_Input)
        {
            yield return new WaitForSeconds(m_MachineData.TimeToProduce);
            int amount = m_Input.FetchResource(1);
        }
    }
    IEnumerator MachineHaulted()
    {
        while(!m_Input)
        {
            yield return new WaitForSeconds(m_MachineData.MachineHaltCheck);
        }
        
        SetMachineState(MachineState.Working);
    }

    public override void SetMachineState(MachineState _state)
    {
        if(State == _state) return;

        switch(_state)
        {
            case MachineState.Inactive:
                StopCoroutine(m_MachineWorkingCoroutine);
                break;
            case MachineState.Working:
                m_MachineWorkingCoroutine = StartCoroutine(MachineWork());
                break;
            case MachineState.Halted:
                m_MachineHaultedCoroutine = StartCoroutine(MachineHaulted());
                break;
        }
    }
}
