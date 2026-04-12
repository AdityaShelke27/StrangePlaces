using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class NodeMachineInstance : MachineInstance
{
    [SerializeField] private NodeMachine m_MachineData;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private ResourceNodeInstance m_Input;
    [SerializeField] private TMP_Text m_MachineStateText;
    Coroutine m_MachineWorkingCoroutine;
    Coroutine m_MachineHaultedCoroutine;
    [SerializeField] private ItemSlot m_Output = new();

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
        SetMachineState(MachineState.Halted);
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

            if(m_Output.item == null)
            {
                m_Output = new(m_Input.GetResourceNodeData().ResourceYield, amount);
            }
            else
            {
                m_Output.amount += amount;
            }
        }

        SetMachineState(MachineState.Halted);
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
                if (m_MachineWorkingCoroutine != null) StopCoroutine(m_MachineWorkingCoroutine);
                m_MachineWorkingCoroutine = StartCoroutine(MachineWork());
                break;
            case MachineState.Halted:
                if (m_MachineHaultedCoroutine != null) StopCoroutine(m_MachineHaultedCoroutine);
                m_MachineHaultedCoroutine = StartCoroutine(MachineHaulted());
                break;
        }

        m_MachineStateText.text = "Machine State:" + _state.ToString();
    }
}
