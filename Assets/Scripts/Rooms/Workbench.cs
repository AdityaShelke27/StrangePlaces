using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
	public static Workbench Instance;

	[SerializeField] GameObject m_WorkbenchPanelUI;
	[SerializeField] GameObject m_WorkbenchCraftButtonPrefab;
	[SerializeField] GameObject m_ResourceRequirementPrefab;
	[SerializeField] Transform m_ButtonContentParent;
	[SerializeField] CraftObject[] m_CraftItems;
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
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (!m_AreResourcesAssigned)
		{
			Debug.LogWarning("Construction resources art not yet assigned");
			return;
		}

		CheckAvailableResources();
		m_WorkbenchPanelUI.SetActive(true);
	}
	private void AssignMachineCraftingData()
	{
		m_RequiredResourcesTexts = new TMP_Text[m_CraftItems.Length][];
		for (int i = 0; i < m_CraftItems.Length; i++)
		{
			GameObject _objButton = Instantiate(m_WorkbenchCraftButtonPrefab, m_ButtonContentParent);
			MachineCraftingButtonManager _buttonManager = _objButton.GetComponent<MachineCraftingButtonManager>();
			_buttonManager.SetName(m_CraftItems[i].CraftItem.itemName);
			_buttonManager.SetImage(m_CraftItems[i].CraftItem.itemImage);

			ResourceRequirement[] _requirements = m_CraftItems[i].ResourceRequirements;
			m_RequiredResourcesTexts[i] = new TMP_Text[_requirements.Length];

			for (int j = 0; j < _requirements.Length; j++)
			{
				ResourceRequirement _requirement = _requirements[j];
				GameObject _objSlot = Instantiate(m_ResourceRequirementPrefab, _buttonManager.GetResourceInputParent());
				ResourceRequirementManager _objResourceManager = _objSlot.GetComponent<ResourceRequirementManager>();
				_objResourceManager.AssignResourceImageAndAmount(_requirement.item.itemImage, _requirement.amount.ToString());

				m_RequiredResourcesTexts[i][j] = _objResourceManager.GetAmountText();
			}
			int _idx = i;
			_objButton.GetComponent<Button>().onClick.AddListener(() => ConstructMachine(m_CraftItems[_idx]));
		}
		m_AreResourcesAssigned = true;
	}
	void CheckAvailableResources()
	{
		for (int i = 0; i < m_CraftItems.Length; i++)
		{
			ResourceRequirement[] _requirements = m_CraftItems[i].ResourceRequirements;
			for (int j = 0; j < _requirements.Length; j++)
			{
				ResourceRequirement _requirement = _requirements[j];
				m_RequiredResourcesTexts[i][j].color = ResourceTracker.Instance.SearchResourceAvailable(_requirement.item as StorableItem, _requirement.amount) ? Color.green : Color.red;
			}
		}
	}
	void ConstructMachine(CraftObject _craftMachine)
	{
		if (_craftMachine.CraftItem.itemID == "research-point") 
		{
			AddResearchPoint(_craftMachine);
			return; 
		}
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
	void AddResearchPoint(CraftObject _craftMachine)
	{
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
		CheckAvailableResources();
		PlayerStatsManager.Instance.AddResearchPoints(_craftMachine.CraftAmount);
	}
	public void ClosePanel() => m_WorkbenchPanelUI.SetActive(false);
}
