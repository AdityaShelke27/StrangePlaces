using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceMachineInstance : MachineInstance
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private ResourceMachine m_MachineData;
    [SerializeField] private int m_SelectedRecipeIdx;
    [SerializeField] private TMP_Text m_MachineStateText;
    private ItemSlot[] m_Inputs;
    private ItemSlot[] m_Outputs;
    Coroutine m_MachineWorkingCoroutine;
    Coroutine m_MachineHaultedCoroutine;

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
        m_MachineData = data as ResourceMachine;
        m_SpriteRenderer.sprite = m_MachineData.itemImage;
        GetComponent<BoxCollider2D>().size = m_MachineData.Size;
        m_Inputs = new ItemSlot[m_MachineData.InputSlots];
        m_Outputs = new ItemSlot[m_MachineData.OutputSlots];

        for (int i = 0; i < m_Inputs.Length; i++)
        {
            m_Inputs[i] = new();
        }
        for (int i = 0; i < m_Outputs.Length; i++)
        {
            m_Outputs[i] = new();
        }
        SetMachineState(MachineState.Halted);
    }

    public override void StartMachine()
    {
        if (!m_Inputs[0].item)
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

        while (m_Inputs[0].item)
        {
            yield return new WaitForSeconds(m_MachineData.TimeToProduce);

            ResourceRecipeData data = m_MachineData.RecipeData[m_SelectedRecipeIdx];
            if (m_Inputs[0].item == data.Input[0].Resource && m_Inputs[0].amount > data.Input[0].amount)
            {
                if (m_Outputs[0].item == null)
                {
                    m_Outputs[0] = new(data.Output[0].Resource, data.Output[0].amount);
                }
                else
                {
                    m_Outputs[0].amount += data.Output[0].amount;
                }
            }
            else
            {
                SetMachineState(MachineState.Halted);
            }
        }
    }
    IEnumerator MachineHaulted()
    {
        while (!m_Inputs[0].item)
        {
            yield return new WaitForSeconds(m_MachineData.MachineHaltCheck);
        }

        SetMachineState(MachineState.Working);
    }

    public override void SetMachineState(MachineState _state)
    {
        if (State == _state) return;

        switch (_state)
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
