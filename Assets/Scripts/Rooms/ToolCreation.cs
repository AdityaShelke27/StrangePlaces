using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolCreation : MonoBehaviour
{
	public static ToolCreation Instance;

	[SerializeField] GameObject m_ToolPanelUI;
	[SerializeField] GameObject m_ToolCraftButtonPrefab;
	[SerializeField] GameObject m_ResourceRequirementPrefab;
	[SerializeField] Transform m_ButtonContentParent;
	[SerializeField] CraftObject[] m_CraftTools;
	bool m_AreResourcesAssigned = false;
	TMP_Text[][] m_RequiredResourcesTexts;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}
	private void Start()
	{
		ClosePanel();
		AssignMachineCraftingData();
	}
	private void OnMouseUpAsButton()
	{
		if (InputManager.IsADrag()) return;

		StartCoroutine(Constant.DelayExecute(() =>
		{
			if (InputManager.IsPointerOverUI()) return;
			if (!m_AreResourcesAssigned)
			{
				Debug.LogWarning("Construction resources art not yet assigned");
				return;
			}

			CheckAvailableResources();
			m_ToolPanelUI.SetActive(true);
		}));
	}
	private void AssignMachineCraftingData()
	{
		m_RequiredResourcesTexts = new TMP_Text[m_CraftTools.Length][];
		for (int i = 0; i < m_CraftTools.Length; i++)
		{
			GameObject _objButton = Instantiate(m_ToolCraftButtonPrefab, m_ButtonContentParent);
			MachineCraftingButtonManager _buttonManager = _objButton.GetComponent<MachineCraftingButtonManager>();
			_buttonManager.SetName(m_CraftTools[i].CraftItem.itemName);
			_buttonManager.SetImage(m_CraftTools[i].CraftItem.itemImage);

			ResourceRequirement[] _requirements = m_CraftTools[i].ResourceRequirements;
			m_RequiredResourcesTexts[i] = new TMP_Text[_requirements.Length];

			for (int j = 0; j < _requirements.Length; j++)
			{
				ResourceRequirement _requirement = _requirements[j];
				GameObject _objSlot = Instantiate(m_ResourceRequirementPrefab, _buttonManager.GetResourceInputParent());
				ResourceRequirementManager _objResourceManager = _objSlot.GetComponent<ResourceRequirementManager>();
				_objResourceManager.AssignResourceImageNameAndAmount(_requirement.item.itemImage, _requirement.item.itemName, _requirement.amount.ToString());

				m_RequiredResourcesTexts[i][j] = _objResourceManager.GetAmountText();
			}
			int _idx = i;
			_objButton.GetComponent<Button>().onClick.AddListener(() => ConstructMachine(m_CraftTools[_idx]));
		}
		m_AreResourcesAssigned = true;
	}
	void CheckAvailableResources()
	{
		for (int i = 0; i < m_CraftTools.Length; i++)
		{
			ResourceRequirement[] _requirements = m_CraftTools[i].ResourceRequirements;
			for (int j = 0; j < _requirements.Length; j++)
			{
				ResourceRequirement _requirement = _requirements[j];

				int _amount = ResourceTracker.Instance.SearchResourceAvailableAmount(_requirement.item as StorableItem, _requirement.amount);
				m_RequiredResourcesTexts[i][j].text = $"{_amount} / {_requirement.amount}";
				m_RequiredResourcesTexts[i][j].color = _amount >= _requirement.amount ? Constant.COLOR_RESOURCE_AVAILABLE : Constant.COLOR_RESOURCE_UNAVAILABLE;
			}
		}
	}
	void ConstructMachine(CraftObject _craftMachine)
	{
		if (!ResourceTracker.Instance.IsItemAddable(_craftMachine.CraftItem, _craftMachine.CraftAmount))
		{
			Debug.LogWarning("Not enough inventory slots available");
			return;
		}

		ResourceRequirement[] _requirements = _craftMachine.ResourceRequirements;
		bool _areResourcesAvailable = true;
		for (int j = 0; j < _requirements.Length; j++)
		{
			if (!ResourceTracker.Instance.SearchResourceAvailable(_requirements[j].item as StorableItem, _requirements[j].amount))
			{
				_areResourcesAvailable = false;
				break;
			}
		}
		if (!_areResourcesAvailable)
		{
			Debug.LogWarning("Not enough resources available in the inventory");
			return;
		}
		for (int j = 0; j < _requirements.Length; j++)
		{
			ResourceTracker.Instance.SearchAndRemoveResource(_requirements[j].item as StorableItem, _requirements[j].amount);
		}
		ResourceTracker.Instance.AddStorableItemToInventory(_craftMachine.CraftItem, _craftMachine.CraftAmount);
		CheckAvailableResources();
	}

	public void ClosePanel() => m_ToolPanelUI.SetActive(false);
}
