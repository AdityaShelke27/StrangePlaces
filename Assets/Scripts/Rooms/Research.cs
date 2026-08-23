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
	[SerializeField] Transform m_MainConnectionsParent;
	[SerializeField] Transform m_RocketConnectionsParent;
	[SerializeField] GameObject m_BazierConnection;
	[SerializeField] GameObject m_ResearchCanvas;

	string m_UnlockedResearch = "";
	string m_DefaultResearchAvailable = "1 2 3";
	Dictionary<int, ResearchNode> m_MainResearchNode_Dict = new();
	Dictionary<int, ResearchNode> m_RocketResearchNode_Dict = new();
	private void Start()
	{
		m_ResearchCanvas.SetActive(false);
		CreateResearchDictionary();

		CreateResearchNodeStatus(m_MainResearchNode_Dict);
		CreateResearchNodeStatus(m_RocketResearchNode_Dict);
		CreateBazierConnections(m_MainResearchNode_Dict, true);
		CreateBazierConnections(m_RocketResearchNode_Dict, false);
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
	void CreateBazierConnections(Dictionary<int, ResearchNode> _researchDict, bool _isMainResearch)
	{
		foreach (int _key in _researchDict.Keys)
		{
			ResearchNodeInfo[] _unlocks = _researchDict[_key].GetResearchNodeInfo().Unlocks;
			RectTransform _nodeTransform = _researchDict[_key].GetComponent<RectTransform>();
			Transform _connectionParent = _isMainResearch ? m_MainConnectionsParent : m_RocketConnectionsParent;

			foreach (ResearchNodeInfo _unlock in _unlocks)
			{
				GameObject _connection = Instantiate(m_BazierConnection, _connectionParent);
				BezierUIConnection _bezier = _connection.GetComponent<BezierUIConnection>();
				_bezier.SetEndpoints(_nodeTransform, _researchDict[_unlock.ID].GetComponent<RectTransform>());
			}
		}
	}
	void CreateResearchDictionary()
	{
		for(int i = 0; i < m_MainResearchContentParent.childCount; i++)
		{
			if(!m_MainResearchContentParent.GetChild(i).TryGetComponent(out ResearchNode _script)) continue;

			m_MainResearchNode_Dict[_script.GetResearchNodeInfo().ID] = _script;
		}
		for (int i = 0; i < m_RocketResearchContentParent.childCount; i++)
		{
			if (!m_RocketResearchContentParent.GetChild(i).TryGetComponent(out ResearchNode _script)) continue;

			m_RocketResearchNode_Dict[_script.GetResearchNodeInfo().ID] = _script;
		}
	}
	void CreateResearchNodeStatus(Dictionary<int, ResearchNode> _researchDict)
	{
		//m_UnlockedResearch = PlayerPrefs.GetString(Constant.PREF_RESEARCHEDNODES, "");

		if(string.IsNullOrEmpty(m_UnlockedResearch))
		{
			string[] _defaults = m_DefaultResearchAvailable.Split();
			foreach(string _id in _defaults)
			{
				int _k = Convert.ToInt32(_id);
				if(_researchDict.ContainsKey(_k)) _researchDict[_k].SetNodeStatus(E_ResearchStatus.Available);
			}
		}
		else
		{
			string[] _unlocked = m_UnlockedResearch.Split();

			foreach(string _id in _unlocked)
			{
				int _numID = Convert.ToInt32(_id);
				if (_researchDict.ContainsKey(_numID)) _researchDict[_numID].SetNodeStatus(E_ResearchStatus.Researched);
			}
			foreach (string _id in _unlocked)
			{
				int _numID = Convert.ToInt32(_id);
				if (!_researchDict.ContainsKey(_numID)) continue;

				ResearchNodeInfo[] _unlocks = _researchDict[_numID].GetResearchNodeInfo().Unlocks;

				foreach(ResearchNodeInfo _unlock in _unlocks)
				{
					if(_researchDict[_unlock.ID].GetNodeStatus() == E_ResearchStatus.Locked)
					{
						_researchDict[_unlock.ID].SetNodeStatus(E_ResearchStatus.Available);
					}
				}
			}
		}
	}
	public void ClosePanel() => m_ResearchCanvas.SetActive(false);
}
