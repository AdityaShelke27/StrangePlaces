using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceMachineInstance : MachineInstance
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private ResourceMachine m_MachineData;
    [SerializeField] private int m_SelectedRecipeIdx;
    [SerializeField] private TMP_Text m_MachineStateText;
    [SerializeField] private Transform m_InputSlotListParent;
    [SerializeField] private Transform m_OutputSlotListParent;
    [SerializeField] private GameObject m_InventorySlotPrefab;
    [SerializeField] private InventorySlot[] m_Inputs;
    [SerializeField] private InventorySlot[] m_Outputs;
    Coroutine m_MachineWorkingCoroutine;
    Coroutine m_MachineHaultedCoroutine;

    public override void Initialize(StorableItem data)
    {
        m_MachineData = data as ResourceMachine;
        m_SpriteRenderer.sprite = m_MachineData.itemImage;
        GetComponent<BoxCollider2D>().size = m_MachineData.Size;

        m_Inputs = new InventorySlot[m_MachineData.InputSlots];
        m_Outputs = new InventorySlot[m_MachineData.OutputSlots];

        for (int i = 0; i < m_Inputs.Length; i++)
        {
	        GameObject _slot = Instantiate(m_InventorySlotPrefab, m_InputSlotListParent);
            m_Inputs[i] = _slot.GetComponent<InventorySlot>();

            int _length = m_MachineData.RecipeData[m_SelectedRecipeIdx].Input.Count;

            for (int j = 0; j < _length; j++)
            {
                m_Inputs[i].AddIncludeItems(m_MachineData.RecipeData[m_SelectedRecipeIdx].Input[j].Resource);
            }
        }
        for (int i = 0; i < m_Outputs.Length; i++)
        {
            GameObject _slot = Instantiate(m_InventorySlotPrefab, m_OutputSlotListParent);
            m_Outputs[i] = _slot.GetComponent<InventorySlot>();

            int _length = m_MachineData.RecipeData[m_SelectedRecipeIdx].Output.Count;

            for (int j = 0; j < _length; j++)
            {
                m_Outputs[i].AddIncludeItems(m_MachineData.RecipeData[m_SelectedRecipeIdx].Output[j].Resource);
            }
        }
        SetMachineState(MachineState.Halted);
    }

    public override void StartMachine()
    {
        if (!m_Inputs[0].GetItem())
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

        while (m_Inputs[0].GetItem())
        {
            yield return new WaitForSeconds(m_MachineData.TimeToProduce);

            ResourceRecipeData data = m_MachineData.RecipeData[m_SelectedRecipeIdx];

            if (m_Inputs[0].GetItem() == data.Input[0].Resource && m_Inputs[0].GetItemAmount() >= data.Input[0].amount)
            {
                m_Inputs[0].SetItemAmount(m_Inputs[0].GetItemAmount() - data.Input[0].amount);
                if(m_Inputs[0].GetItemAmount() == 0)
                {
                    m_Inputs[0].SetItem(null);
                }
                if (m_Outputs[0].GetItem() == null)
                {
                    m_Outputs[0].SetItemSlot(data.Output[0].Resource, data.Output[0].amount);
                }
                else
                {
                    m_Outputs[0].SetItemAmount(m_Outputs[0].GetItemAmount() + data.Output[0].amount);
                }
            }
            else
            {
                SetMachineState(MachineState.Halted);
            }

            Debug.Log("Machine Working");
        }
        SetMachineState(MachineState.Halted);
    }
    IEnumerator MachineHaulted()
    {
        while (!m_Inputs[0].GetItem())
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
