using System.Collections;
using TMPro;
using UnityEngine;

public class NodeMachineInstance : MachineInstance
{
    [SerializeField] private NodeMachine m_MachineData;
    [SerializeField] private ResourceNodeInstance m_Input;
    [SerializeField] private TMP_Text m_MachineStateText;
    Coroutine m_MachineWorkingCoroutine;
    Coroutine m_MachineHaultedCoroutine;
    [SerializeField] private InventorySlot[] m_Outputs;

    public override void Initialize(StorableItem data)
    {
        m_MachineData = data as NodeMachine;
        m_SpriteRenderer.sprite = m_MachineData.itemImage;
        GetComponent<BoxCollider2D>().size = m_MachineData.Size;

        m_Outputs = new InventorySlot[m_MachineData.OutputSlots];
        for (int i = 0; i < m_Outputs.Length; i++)
        {
            GameObject _slot = Instantiate(m_InventorySlotPrefab, m_OutputSlotListParent);
            m_Outputs[i] = _slot.GetComponent<InventorySlot>();
        }

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
        while(m_Input)
        {
            yield return new WaitForSeconds(m_MachineData.TimeToProduce);
            int _amount = m_Input.FetchResource(1);

            if (m_Outputs[0].GetItem() == null)
            {
                m_Outputs[0].SetItemSlot(m_Input.GetResourceNodeData().ResourceYield, _amount);
            }
            else
            {
                int _sumAmount = m_Outputs[0].GetItemAmount() + _amount;
                m_Outputs[0].SetItemAmount(_sumAmount);
                if (_sumAmount >= m_Outputs[0].GetItem().StackableAmount)
                {
                    Debug.Log("Machine should hault");
                    SetMachineState(MachineState.Halted);
                }
            }
        }

        SetMachineState(MachineState.Halted);
    }
    IEnumerator MachineHaulted()
    {
        while(!m_Input || (m_Outputs[0].GetItem() != null && m_Outputs[0].GetItemAmount() >= m_Outputs[0].GetItem().StackableAmount))
        {
            yield return new WaitForSeconds(m_MachineData.MachineHaltCheck);
        }
        
        SetMachineState(MachineState.Working);
    }

    public override void SetMachineState(MachineState _state)
    {
        if(State == _state) return;
        State = _state;

        if (m_MachineWorkingCoroutine != null) StopCoroutine(m_MachineWorkingCoroutine);
        if (m_MachineHaultedCoroutine != null) StopCoroutine(m_MachineHaultedCoroutine);

        switch (_state)
        {
            case MachineState.Inactive:
                break;
            case MachineState.Working:
                m_MachineWorkingCoroutine = StartCoroutine(MachineWork());
                break;
            case MachineState.Halted:
                m_MachineHaultedCoroutine = StartCoroutine(MachineHaulted());
                break;
        }

        m_MachineStateText.text = "Machine State:" + _state.ToString();
    }
}
