using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchNode : MonoBehaviour
{
	static ResearchNode s_SelectedButton;

	[SerializeField] private ResearchNodeInfo m_ResearchNodeInfo;
	private E_ResearchStatus m_ResearchNodeStatus;
	[Header("UI")]
	[SerializeField] Image m_ResearchNodeIcon;
	[SerializeField] TMP_Text m_ResearchNodeTitle;
	[SerializeField] TMP_Text m_ResearchNodeDescription;
	[SerializeField] Transform m_ResourceRequirementParent;
	[SerializeField] GameObject m_ResourceRequirementSlotPrefab;
	[SerializeField] TMP_Text m_ResearchPointText;
	[SerializeField] GameObject m_ResearchButton;

	[SerializeField] int m_UnlocksNeeded;
	[SerializeField] int m_UnlocksCompleted;

	public void SetNodeStatus(E_ResearchStatus _nodeStatus)
	{
		m_ResearchNodeStatus = _nodeStatus;

		// CHANGE UI ACCORDING TO STATUS
		switch(_nodeStatus)
		{
			case E_ResearchStatus.Available:
				GetComponent<Image>().color = Color.green;
				break;
			case E_ResearchStatus.Researched:
				GetComponent<Image>().color = Color.blue;
				m_ResearchButton.SetActive(false);
				break;
			case E_ResearchStatus.Locked:
				GetComponent<Image>().color = Color.black;
				m_ResearchButton.SetActive(false);
				break;
		}
	}
	public void SetupResearchNodeUI()
	{
		m_ResearchNodeIcon.sprite = m_ResearchNodeInfo.Icon;
		m_ResearchNodeTitle.text = m_ResearchNodeInfo.Name;
		m_ResearchNodeDescription.text = m_ResearchNodeInfo.Description;
		m_ResearchPointText.text = m_ResearchNodeInfo.ResearchCost.ToString() + " RP";

		for (int i = 0; i < m_ResearchNodeInfo.ResourceRequirements.Length; i++)
		{
			GameObject _requirementSlot = Instantiate(m_ResourceRequirementSlotPrefab, m_ResourceRequirementParent);
			ResourceRequirementManager _requirementManagerScript = _requirementSlot.GetComponent<ResourceRequirementManager>();
			ResourceRequirement _resourceRequirement = m_ResearchNodeInfo.ResourceRequirements[i];
			_requirementManagerScript.AssignResourceImageNameAndAmount(_resourceRequirement.item.itemImage, _resourceRequirement.item.itemName, _resourceRequirement.amount.ToString());
		}

		m_UnlocksNeeded = m_ResearchNodeInfo.Prerequisites.Length;
	}
	public void SelectButton()
	{
		if(s_SelectedButton)
		{
			s_SelectedButton.SetResearchButtonActive(false);
		}

		s_SelectedButton = this;
		SetResearchButtonActive(true);
	}
	public void SetResearchButtonActive(bool _active)
	{
		m_ResearchButton.SetActive(_active && m_ResearchNodeStatus == E_ResearchStatus.Available);
	}
	public void ResearchButton()
	{
		string _researched = PlayerPrefs.GetString(Constant.PREF_RESEARCHEDNODES, "0");
		_researched = _researched + m_ResearchNodeInfo.ID + " ";
		PlayerPrefs.SetString(Constant.PREF_RESEARCHEDNODES, _researched);
		SetNodeStatus(E_ResearchStatus.Researched);

		Research.Instance.RefreshResearchNodeStatus();
	}
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
