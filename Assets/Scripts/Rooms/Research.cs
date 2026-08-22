using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Research : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] GameObject m_MainResearchPanel;
	[SerializeField] GameObject m_RocketResearchPanel;
	[SerializeField] Transform m_MainResearchContentParent;
	[SerializeField] Transform m_RocketResearchContentParent;
	[SerializeField] Transform m_ConnectionsParent;
	[SerializeField] GameObject m_BazierConnection;
	[SerializeField] GameObject m_ResearchCanvas;

	string m_UnlockedResearch = "";
	string m_DefaultResearchAvailable = "1 2 3";
	Dictionary<int, ResearchNode> m_ResearchNode_Dict = new();
	private void Start()
	{
		m_ResearchCanvas.SetActive(false);
		CreateResearchDictionary();
		CreateResearchNodeStatus();
	}
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		m_ResearchCanvas.SetActive(true);
		SelectMainResearch();
	}

	public void SelectMainResearch()
	{
		m_MainResearchPanel.SetActive(true);
		m_RocketResearchPanel.SetActive(false);
	}
	public void SelectRocketResearch()
	{
		m_MainResearchPanel.SetActive(false);
		m_RocketResearchPanel.SetActive(true);
	}
	void CreateBazierConnections()
	{
		foreach (int _key in m_ResearchNode_Dict.Keys)
		{
			ResearchNodeInfo[] _unlocks = m_ResearchNode_Dict[_key].GetResearchNodeInfo().Unlocks;

			GameObject _connection = Instantiate(m_BazierConnection, m_ConnectionsParent);

			foreach(ResearchNodeInfo _unlock in _unlocks)
			{
				
			}
		}
	}
	void CreateResearchDictionary()
	{
		for(int i = 0; i < m_MainResearchContentParent.childCount; i++)
		{
			if(!m_MainResearchContentParent.GetChild(i).TryGetComponent(out ResearchNode _script)) continue;

			m_ResearchNode_Dict[_script.GetResearchNodeInfo().ID] = _script;
		}
		for (int i = 0; i < m_RocketResearchContentParent.childCount; i++)
		{
			if (!m_RocketResearchContentParent.GetChild(i).TryGetComponent(out ResearchNode _script)) continue;

			m_ResearchNode_Dict[_script.GetResearchNodeInfo().ID] = _script;
		}
	}
	void CreateResearchNodeStatus()
	{
		//m_UnlockedResearch = PlayerPrefs.GetString(Constant.PREF_RESEARCHEDNODES, "");

		if(string.IsNullOrEmpty(m_UnlockedResearch))
		{
			string[] _defaults = m_DefaultResearchAvailable.Split();
			foreach(string _id in _defaults)
			{
				m_ResearchNode_Dict[Convert.ToInt32(_id)].SetNodeStatus(E_ResearchStatus.Available);
			}
		}
		else
		{
			string[] _unlocked = m_UnlockedResearch.Split();

			foreach(string _id in _unlocked)
			{
				int _numID = Convert.ToInt32(_id);
				m_ResearchNode_Dict[_numID].SetNodeStatus(E_ResearchStatus.Researched);
			}
			foreach (string _id in _unlocked)
			{
				int _numID = Convert.ToInt32(_id);
				ResearchNodeInfo[] _unlocks = m_ResearchNode_Dict[_numID].GetResearchNodeInfo().Unlocks;

				foreach(ResearchNodeInfo _unlock in _unlocks)
				{
					if(m_ResearchNode_Dict[_unlock.ID].GetNodeStatus() == E_ResearchStatus.Locked)
					{
						m_ResearchNode_Dict[_unlock.ID].SetNodeStatus(E_ResearchStatus.Available);
					}
				}
			}
		}
	}
	public void ClosePanel() => m_ResearchCanvas.SetActive(false);
}
