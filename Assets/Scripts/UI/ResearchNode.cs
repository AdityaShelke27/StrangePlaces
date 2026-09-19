using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchNode : MonoBehaviour
{
	public static Action<int> s_ResearchedAction;
	static ResearchNode s_SelectedButton;

	[SerializeField] private ResearchNodeInfo m_ResearchNodeInfo;
	private E_ResearchStatus m_ResearchNodeStatus;
	[Header("Sprites")]
	[SerializeField] Sprite m_ResearchedIcon;
	[SerializeField] Sprite m_LockedIcon;
	[Header("UI")]
	[SerializeField] Image m_ResearchNodeImage;
	[SerializeField] Image m_ResearchStatusImage;
	[SerializeField] TMP_Text m_ResearchNodeTitle;
	[SerializeField] TMP_Text m_ResearchNodeDescription;
	[SerializeField] Transform m_ResourceRequirementParent;
	[SerializeField] GameObject m_ResourceRequirementSlotPrefab;
	[SerializeField] TMP_Text m_ResearchPointText;
	[SerializeField] GameObject m_ResearchButton;
	[SerializeField] GameObject m_MiddleSection;
	[SerializeField] GameObject m_BottomSection;

	ResourceRequirementManager[] m_ResourceRequirementSlots;

	[SerializeField] int m_UnlocksNeeded;
	[SerializeField] int m_UnlocksCompleted;

	public void SetNodeStatus(E_ResearchStatus _nodeStatus)
	{
		m_ResearchNodeStatus = _nodeStatus;

		// CHANGE UI ACCORDING TO STATUS
		switch(_nodeStatus)
		{
			case E_ResearchStatus.Available:
				GetComponent<Image>().color = Constant.RESEARCH_STATUS_AVAILABLE;
				m_ResearchStatusImage.enabled = false;
				m_MiddleSection.SetActive(true);
				m_BottomSection.SetActive(true);
				m_ResearchButton.SetActive(true);
				break;
			case E_ResearchStatus.Researched:
				GetComponent<Image>().color = Constant.RESEARCH_STATUS_RESEARCHED;
				m_ResearchStatusImage.sprite = m_ResearchedIcon;
				m_ResearchStatusImage.enabled = true;
				m_MiddleSection.SetActive(false);
				m_BottomSection.SetActive(false);
				break;
			case E_ResearchStatus.Locked:
				m_ResearchStatusImage.sprite = m_LockedIcon;
				m_ResearchStatusImage.enabled = true;
				GetComponent<Image>().color = Constant.RESEARCH_STATUS_LOCKED;
				m_MiddleSection.SetActive(false);
				m_BottomSection.SetActive(false);
				m_ResearchButton.SetActive(false);
				break;
		}
	}
	public void SetupResearchNodeUI()
	{
		m_ResearchNodeImage.sprite = m_ResearchNodeInfo.Icon;
		m_ResearchNodeTitle.text = m_ResearchNodeInfo.Name;
		m_ResearchNodeDescription.text = m_ResearchNodeInfo.Description;
		m_ResearchPointText.text = m_ResearchNodeInfo.ResearchCost.ToString() + " RP";

		m_ResourceRequirementSlots = new ResourceRequirementManager[m_ResearchNodeInfo.ResourceRequirements.Length];
		for (int i = 0; i < m_ResearchNodeInfo.ResourceRequirements.Length; i++)
		{
			GameObject _requirementSlot = Instantiate(m_ResourceRequirementSlotPrefab, m_ResourceRequirementParent);
			m_ResourceRequirementSlots[i] = _requirementSlot.GetComponent<ResourceRequirementManager>();
			ResourceRequirement _resourceRequirement = m_ResearchNodeInfo.ResourceRequirements[i];
			m_ResourceRequirementSlots[i].AssignResourceImageNameAndAmount(_resourceRequirement.item.itemImage, _resourceRequirement.item.itemName, _resourceRequirement.amount.ToString());
		}

		m_UnlocksNeeded = m_ResearchNodeInfo.Prerequisites.Length;
	}
	public void SelectButton()
	{
		if(s_SelectedButton)
		{
			if (s_SelectedButton == this)
			{
				s_SelectedButton.SetResearchButtonActive(!m_MiddleSection.activeSelf);
				return;
			}
			else s_SelectedButton.SetResearchButtonActive(false);
		}

		s_SelectedButton = this;
		SetResearchButtonActive(true);
	}
	public void SetResearchButtonActive(bool _active)
	{
		if(m_ResearchNodeStatus == E_ResearchStatus.Locked)
		{
			m_MiddleSection.SetActive(_active);
			m_BottomSection.SetActive(_active);
		}
	}
	public void ResearchButton()
	{
		if(PlayerStatsManager.Instance.GetResearchPoints() < m_ResearchNodeInfo.ResearchCost)
		{
			Debug.LogWarning("Not enough research points");
			return;
		}

		ResourceRequirement[] _requirements = m_ResearchNodeInfo.ResourceRequirements;
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
		PlayerStatsManager.Instance.AddResearchPoints(-m_ResearchNodeInfo.ResearchCost);


		if (!PlayerPrefs.HasKey(Constant.PREF_RESEARCHEDNODES))
		{
			PlayerPrefs.SetString(Constant.PREF_RESEARCHEDNODES, "0 ");
		}

		int _researchID = m_ResearchNodeInfo.ID;
		string _researched = PlayerPrefs.GetString(Constant.PREF_RESEARCHEDNODES, "0 ");
		_researched = _researched + _researchID + " ";
		PlayerPrefs.SetString(Constant.PREF_RESEARCHEDNODES, _researched);
		SetNodeStatus(E_ResearchStatus.Researched);

		Research.Instance.RefreshResearchNodeStatus();
		ItemDatabase.Instance.AddResearchID(_researchID);
		s_ResearchedAction?.Invoke(_researchID);
	}
	public void SetResearchPointAvailableStatus(bool _areResearchPointsAvailable)
	{
		m_ResearchPointText.color = _areResearchPointsAvailable ? Color.green : Color.red;
	}
	public ResourceRequirementManager GetResourceRequirementSlot(int i) => m_ResourceRequirementSlots[i];
	public void SetUnlocksCompleted(int _val) => m_UnlocksCompleted = _val;
	public int GetUnlocksCompleted() => m_UnlocksCompleted;
	public bool IsUnlocked() 
	{
		if (m_UnlocksNeeded == 0) return true;

		return m_UnlocksCompleted / m_UnlocksNeeded == 1; 
	}
	public ResearchNodeInfo GetResearchNodeInfo() => m_ResearchNodeInfo;
	public E_ResearchStatus GetNodeStatus() => m_ResearchNodeStatus;
}
