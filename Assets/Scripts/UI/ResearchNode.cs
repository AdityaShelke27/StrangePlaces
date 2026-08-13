using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchNode : MonoBehaviour
{
	//public static Action<int, E_ResearchStatus> s_SetResearchNodeStatus;

	[SerializeField] private ResearchNodeInfo m_ResearchNodeInfo;
	private E_ResearchStatus m_ResearchNodeStatus;
	[Header("UI")]
	[SerializeField] Image m_ResearchNodeIcon;
	[SerializeField] TMP_Text m_ResearchNodeTitle;
	[SerializeField] TMP_Text m_ResearchNodeDescription;
	[SerializeField] Transform m_ResourceRequirementParent;
	[SerializeField] GameObject m_ResourceRequirementSlotPrefab;
	[SerializeField] TMP_Text m_ResearchPointText;

	void Start()
	{
		SetupResearchNodeUI();
	}
	//private void OnEnable()
	//{
	//	s_SetResearchNodeStatus += SetResearchNodeStatus;
	//}
	//private void OnDisable()
	//{
	//	s_SetResearchNodeStatus -= SetResearchNodeStatus;
	//}
	//void SetResearchNodeStatus(int _id, E_ResearchStatus _nodeStatus)
	//{
	//	if (_id != m_ResearchNodeInfo.ID) return;

	//	SetNodeStatus(_nodeStatus);
	//}
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
				break;
			case E_ResearchStatus.Locked:
				GetComponent<Image>().color = Color.black;
				break;
		}
	}
	void SetupResearchNodeUI()
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
	}
	public ResearchNodeInfo GetResearchNodeInfo() => m_ResearchNodeInfo;
}
