using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceMachineInstance : MachineInstance
{
	[SerializeField] private ResourceMachine m_MachineData;
	[SerializeField] private int m_SelectedRecipeIdx;
	[SerializeField] private Transform m_InputSlotListParent;
	[SerializeField] private GameObject m_InputSocket;
	[SerializeField] private GameObject m_OutputSocket;
	[SerializeField] private InventorySlot[] m_Inputs;
	[SerializeField] private InventorySlot[] m_Outputs;
	Coroutine m_MachineWorkingCoroutine;
	Coroutine m_MachineHaultedCoroutine;

	List<StorableItem> m_AddableInputItems;

	bool m_IsOutputAbstract = false;

	private void OnEnable()
	{
		ConveyorManager.s_StartConveyorMode += StartConveyorMode;
		ConveyorManager.s_EndConveyorMode += EndConveyorMode;
	}
	private void OnDisable()
	{
		ConveyorManager.s_StartConveyorMode -= StartConveyorMode;
		ConveyorManager.s_EndConveyorMode -= EndConveyorMode;
	}
	public override void Initialize(StorableItem data)
	{
		m_MachineData = data as ResourceMachine;
		m_SpriteRenderer.sprite = m_MachineData.itemImage;
		GetComponent<BoxCollider2D>().size = m_MachineData.Size;

		m_Inputs = new InventorySlot[m_MachineData.InputSlots];
		m_Outputs = new InventorySlot[m_MachineData.OutputSlots];

		m_AddableInputItems = new();
		for (int i = 0; i < m_Inputs.Length; i++)
		{
			GameObject _slot = Instantiate(m_InventorySlotPrefab, m_InputSlotListParent);
			m_Inputs[i] = _slot.GetComponent<InventorySlot>();

			for(int j = 0; j < m_MachineData.RecipeData.Length; j++)
			{
				int _length = m_MachineData.RecipeData[j].Input.Count;

				for (int k = 0; k < _length; k++)
				{
					m_Inputs[i].AddIncludeItems(m_MachineData.RecipeData[j].Input[k].Resource);
					m_AddableInputItems.Add(m_MachineData.RecipeData[j].Input[k].Resource);
				}
			}
		}
		if(m_MachineData.itemID.Equals("research-station") || m_MachineData.itemID.Equals("bio-reactor"))
		{
			m_IsOutputAbstract = true;
		}
		else
		{
			for (int i = 0; i < m_Outputs.Length; i++)
			{
				GameObject _slot = Instantiate(m_InventorySlotPrefab, m_OutputSlotListParent);
				m_Outputs[i] = _slot.GetComponent<InventorySlot>();

				for (int j = 0; j < m_MachineData.RecipeData.Length; j++)
				{
					int _length = m_MachineData.RecipeData[j].Output.Count;

					for (int k = 0; k < _length; k++)
					{
						m_Outputs[i].AddIncludeItems(m_MachineData.RecipeData[j].Output[k].Resource);
					}
				}
			}
		}

		m_InventoryPanel.SetActive(false);
		m_MachineNameText.text = m_MachineData.itemName;
		m_MachineIconImage.sprite = m_MachineData.itemImage;

		m_InputSocket.transform.localPosition = m_MachineData.ConveyorInputPos;
		m_OutputSocket.transform.localPosition = m_MachineData.ConveyorOutputPos;

		m_InputSocket.SetActive(false);
		m_OutputSocket.SetActive(false);

		SetMachineState(E_MachineState.Halted);
	}

	public override void StartMachine()
	{
		if (!m_Inputs[0].GetItem())
		{
			Debug.LogWarning("Input Empty");
			SetMachineState(E_MachineState.Halted);
			return;
		}
		SetMachineState(E_MachineState.Working);
	}

	IEnumerator MachineWork()
	{
		Debug.Log("Machine Working");
		while (m_Inputs[0].GetItem() && PlayerStatsManager.Instance.GetElectricity() >= m_MachineData.ElectricityConsumption)
		{
			yield return new WaitForSeconds(m_MachineData.TimeToProduce);

			SelectRecipeFromItem(m_Inputs[0].GetItem().itemID);
			ResourceRecipeData data = m_MachineData.RecipeData[m_SelectedRecipeIdx];

			bool _isResearched = true;
			foreach(ResourceAmount _resAmt in data.Output)
			{
				if(!ItemDatabase.Instance.DoesItemIDExistInResearch(_resAmt.Resource.itemID))
				{
					_isResearched = false;
					Debug.LogWarning("Output Item not researched");
					break;
				}

			}
			if (!_isResearched) continue;

			if (m_Inputs[0].GetItem() == data.Input[0].Resource && m_Inputs[0].GetItemAmount() >= data.Input[0].amount)
			{
				m_Inputs[0].AddItemAmount(-data.Input[0].amount);
				if(m_Inputs[0].GetItemAmount() == 0) m_Inputs[0].SetItem(null);

				if (m_IsOutputAbstract)
				{
					string _outputResourceID = data.Output[0].Resource.itemID;
					if(_outputResourceID.Equals("research-point")) PlayerStatsManager.Instance.AddResearchPoints(data.Output[0].amount);
					else PlayerStatsManager.Instance.AddElectricity(data.Output[0].amount);
				}
				else
				{
					if (m_Outputs[0].GetItem() == null)
					{
						m_Outputs[0].SetItemSlot(data.Output[0].Resource, data.Output[0].amount);
					}
					else
					{
						int _sumAmount = m_Outputs[0].GetItemAmount() + data.Output[0].amount;
						m_Outputs[0].SetItemAmount(_sumAmount);
						if (_sumAmount >= m_Outputs[0].GetItem().StackableAmount)
						{
							Debug.Log("Machine should hault");
							SetMachineState(E_MachineState.Halted);
						}
					}
				}
			}
			else
			{
				SetMachineState(E_MachineState.Halted);
			}
			PlayerStatsManager.Instance.AddElectricity(-m_MachineData.ElectricityConsumption);
		}
		SetMachineState(E_MachineState.Halted);
	}
	IEnumerator MachineHaulted()
	{
		if(m_IsOutputAbstract)
		{
			while (!m_Inputs[0].GetItem() || PlayerStatsManager.Instance.GetElectricity() < m_MachineData.ElectricityConsumption)
			{
				yield return new WaitForSeconds(m_MachineData.MachineHaltCheck);
			}
		}
		else
		{
			while (!m_Inputs[0].GetItem() || PlayerStatsManager.Instance.GetElectricity() < m_MachineData.ElectricityConsumption || (m_Outputs[0].GetItem() != null && m_Outputs[0].GetItemAmount() >= m_Outputs[0].GetItem().StackableAmount))
			{
				yield return new WaitForSeconds(m_MachineData.MachineHaltCheck);
			}
		}
			
		SetMachineState(E_MachineState.Working);
	}

	public override void SetMachineState(E_MachineState _state)
	{
		if (State == _state) return;
		State = _state;

		if (m_MachineWorkingCoroutine != null) StopCoroutine(m_MachineWorkingCoroutine);
		if (m_MachineHaultedCoroutine != null) StopCoroutine(m_MachineHaultedCoroutine);

		switch (_state)
		{
			case E_MachineState.Inactive:
				break;
			case E_MachineState.Working:
				m_MachineWorkingCoroutine = StartCoroutine(MachineWork());
				break;
			case E_MachineState.Halted:
				m_MachineHaultedCoroutine = StartCoroutine(MachineHaulted());
				break;
		}

		m_MachineStateRenderer.sprite = m_MachineStateIcons[_state == E_MachineState.Working ? 1 : 0];
	}

	void SelectRecipeFromItem(string _id)
	{
		for(int i = 0; i < m_MachineData.RecipeData.Length; i++)
		{
			ResourceRecipeData _data = m_MachineData.RecipeData[i];
			foreach (ResourceAmount _resourceAmt in _data.Input)
			{
				if(_resourceAmt.Resource.itemID == _id)
				{
					m_SelectedRecipeIdx = i;
					return;
				}
			}
		}
	}
	public override InventorySlot[] GetInputSlots() => m_Inputs;
	public override InventorySlot[] GetOutputSlots()
	{
		if(!m_IsOutputAbstract) return m_Outputs;
		else return null;
	}
	public override StorableItem GetCurrentResourceOutput()
	{
		return m_MachineData.RecipeData[m_SelectedRecipeIdx].Output[0].Resource;
	}
	public override bool IsItemAddable(StorableItem _item)
	{
		return m_AddableInputItems.Contains(_item);
	}

	public override void EnterConveyorMode()
	{
		m_InventoryPanel.SetActive(false);

		ConveyorManager.s_StartConveyorMode?.Invoke();
	}

	protected override void StartConveyorMode()
	{
		m_InputSocket.SetActive(!m_IsOutputAbstract);
		m_OutputSocket.SetActive(true);
	}
	protected override void EndConveyorMode()
	{
		m_InputSocket.SetActive(false);
		m_OutputSocket.SetActive(false);
	}
}
