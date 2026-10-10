using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RocketConstructionWorkbench : MonoBehaviour
{
	public static RocketConstructionWorkbench Instance;

	[SerializeField] GameObject m_RCWorkbenchUI;
	[SerializeField] GameObject m_RCPartsButtonPrefab;
	[SerializeField] GameObject m_ResourceRequirementPrefab;
	[SerializeField] Transform m_ButtonContentParent;
	[SerializeField] CraftObject[] m_CraftParts;
	List<GameObject> m_RCPartsButtons = new();
	bool m_AreResourcesAssigned = false;
	TMP_Text[][] m_RequiredResourcesTexts;

	private void OnEnable()
	{
		ResearchNode.s_ResearchedAction += UnlockRCPartResearched;
	}
	private void OnDisable()
	{
		ResearchNode.s_ResearchedAction -= UnlockRCPartResearched;
	}

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}
	private void Start()
	{
		ClosePanel();
		AssignRCPartsData();
	}
	private void OnMouseUpAsButton()
	{
		if (InputManager.IsADrag()) return;

		StartCoroutine(Constant.DelayExecute(() =>
		{
			if (InputManager.IsPointerOverUI()) return;
			if (!m_AreResourcesAssigned)
			{
				Debug.LogWarning("Construction resources are not yet assigned");
				return;
			}

			CheckAvailableResources();
			m_RCWorkbenchUI.SetActive(true);
		}));
	}
	private void AssignRCPartsData()
	{
		m_RequiredResourcesTexts = new TMP_Text[m_CraftParts.Length][];
		for (int i = 0; i < m_CraftParts.Length; i++)
		{
			GameObject _objButton = Instantiate(m_RCPartsButtonPrefab, m_ButtonContentParent);
			m_RCPartsButtons.Add(_objButton);
			MachineCraftingButtonManager _buttonManager = _objButton.GetComponent<MachineCraftingButtonManager>();
			_buttonManager.SetName(m_CraftParts[i].CraftItem.itemName);
			_buttonManager.SetImage(m_CraftParts[i].CraftItem.itemImage);

			ResourceRequirement[] _requirements = m_CraftParts[i].ResourceRequirements;
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
			Button _objButtonComp = _objButton.GetComponent<Button>();
			_objButtonComp.onClick.AddListener(() => ConstructRCParts(m_CraftParts[_idx]));

			GameObject _notResearched = _objButton.transform.Find("NotResearchedPanel").gameObject;
			_notResearched.SetActive(!ItemDatabase.Instance.DoesItemIDExistInResearch(m_CraftParts[i].CraftItem.itemID));
			_objButtonComp.interactable = !_notResearched.activeSelf;
			if (!_notResearched.activeSelf)
			{
				Debug.LogWarning("Machine not researched");
			}
		}
		m_AreResourcesAssigned = true;
	}
	void CheckAvailableResources()
	{
		for (int i = 0; i < m_CraftParts.Length; i++)
		{
			ResourceRequirement[] _requirements = m_CraftParts[i].ResourceRequirements;
			for (int j = 0; j < _requirements.Length; j++)
			{
				ResourceRequirement _requirement = _requirements[j];

				int _amount = ResourceTracker.Instance.SearchResourceAvailableAmount(_requirement.item as StorableItem, _requirement.amount);
				m_RequiredResourcesTexts[i][j].text = $"{_amount} / {_requirement.amount}";
				m_RequiredResourcesTexts[i][j].color = _amount >= _requirement.amount ? Constant.COLOR_RESOURCE_AVAILABLE : Constant.COLOR_RESOURCE_UNAVAILABLE;
			}
		}
	}
	void ConstructRCParts(CraftObject _craftRCParts)
	{
		if (!ResourceTracker.Instance.IsItemAddable(_craftRCParts.CraftItem, _craftRCParts.CraftAmount))
		{
			Debug.LogWarning("Not enough inventory slots available");
			return;
		}

		ResourceRequirement[] _requirements = _craftRCParts.ResourceRequirements;
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
		ResourceTracker.Instance.AddStorableItemToInventory(_craftRCParts.CraftItem, _craftRCParts.CraftAmount);
		CheckAvailableResources();
	}

	void UnlockRCPartResearched(int _id)
	{
		for (int i = 0; i < m_CraftParts.Length; i++)
		{
			bool _isResearched = ItemDatabase.Instance.DoesItemIDExistInResearch(m_CraftParts[i].CraftItem.itemID);

			GameObject _notResearched = m_RCPartsButtons[i].transform.Find("NotResearchedPanel").gameObject;
			_notResearched.SetActive(!_isResearched);
			m_RCPartsButtons[i].GetComponent<Button>().interactable = !_notResearched.activeSelf;
		}
	}
	public void ClosePanel() => m_RCWorkbenchUI.SetActive(false);
}
