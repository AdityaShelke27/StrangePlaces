using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Research : MonoBehaviour
{
	public static Research Instance;

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

	Dictionary<int, ResearchNode> m_ResearchNode_Dict = new();

	private void Awake()
	{
		if (Instance == null) Instance = this;
	}
	private void Start()
	{
		m_ResearchCanvas.SetActive(false);
		CreateResearchDictionary();
		CreateResearchNodeStatus();
		CreateBazierConnections();
	}
	private void OnMouseDown()
	{
		StartCoroutine(DelayExecute());
	}
	IEnumerator DelayExecute()
	{
		yield return null;

		if (EventSystem.current.IsPointerOverGameObject()) yield break;

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
			RectTransform _nodeTransform = m_ResearchNode_Dict[_key].GetComponent<RectTransform>();
			Transform _connectionParent = _nodeTransform.parent == m_MainResearchContentParent ? m_MainConnectionsParent : m_RocketConnectionsParent;

			foreach (ResearchNodeInfo _unlock in _unlocks)
			{
				GameObject _connection = Instantiate(m_BazierConnection, _connectionParent);
				BezierUIConnection _bezier = _connection.GetComponent<BezierUIConnection>();
				_bezier.SetEndpoints(_nodeTransform, m_ResearchNode_Dict[_unlock.ID].GetComponent<RectTransform>());
			}
		}
	}
	void CreateResearchDictionary()
	{
		for (int i = 0; i < m_MainResearchContentParent.childCount; i++)
		{
			if (!m_MainResearchContentParent.GetChild(i).TryGetComponent(out ResearchNode _script)) continue;

			m_ResearchNode_Dict[_script.GetResearchNodeInfo().ID] = _script;
			_script.SetupResearchNodeUI();
		}
		for (int i = 0; i < m_RocketResearchContentParent.childCount; i++)
		{
			if (!m_RocketResearchContentParent.GetChild(i).TryGetComponent(out ResearchNode _script)) continue;

			m_ResearchNode_Dict[_script.GetResearchNodeInfo().ID] = _script;
			_script.SetupResearchNodeUI();
		}
	}
	void CreateResearchNodeStatus()
	{
		m_UnlockedResearch = PlayerPrefs.GetString(Constant.PREF_RESEARCHEDNODES, "");

		if(!string.IsNullOrEmpty(m_UnlockedResearch))
		{
			string[] _unlocked = m_UnlockedResearch.Split();

			foreach(string _id in _unlocked)
			{
				if (string.IsNullOrEmpty(_id)) continue;

				int _numID = Convert.ToInt32(_id);

				if (m_ResearchNode_Dict.ContainsKey(_numID)) m_ResearchNode_Dict[_numID].SetNodeStatus(E_ResearchStatus.Researched);
			}
		}

		foreach(int _key in m_ResearchNode_Dict.Keys)
		{
			if (m_ResearchNode_Dict[_key].GetNodeStatus() == E_ResearchStatus.Researched) continue;

			ResearchNodeInfo[] _prerequisites = m_ResearchNode_Dict[_key].GetResearchNodeInfo().Prerequisites;
			int _researchComplete = 0;
			foreach (ResearchNodeInfo _prerequisite in _prerequisites)
			{
				if(m_ResearchNode_Dict[_prerequisite.ID].GetNodeStatus() == E_ResearchStatus.Researched) _researchComplete++;
			}
			m_ResearchNode_Dict[_key].SetUnlocksCompleted(_researchComplete);
			if(m_ResearchNode_Dict[_key].IsUnlocked())
			{
				m_ResearchNode_Dict[_key].SetNodeStatus(E_ResearchStatus.Available);
			}
		}
	}
	public void RefreshResearchNodeStatus()
	{
		foreach (int _key in m_ResearchNode_Dict.Keys)
		{
			if (m_ResearchNode_Dict[_key].GetNodeStatus() == E_ResearchStatus.Researched) continue;

			ResearchNodeInfo[] _prerequisites = m_ResearchNode_Dict[_key].GetResearchNodeInfo().Prerequisites;
			int _researchComplete = 0;
			foreach (ResearchNodeInfo _prerequisite in _prerequisites)
			{
				if (m_ResearchNode_Dict[_prerequisite.ID].GetNodeStatus() == E_ResearchStatus.Researched) _researchComplete++;
			}
			m_ResearchNode_Dict[_key].SetUnlocksCompleted(_researchComplete);
			if (m_ResearchNode_Dict[_key].IsUnlocked())
			{
				m_ResearchNode_Dict[_key].SetNodeStatus(E_ResearchStatus.Available);
			}
		}
	}
	public void SetMainResearchNodeStatus(int _id, E_ResearchStatus _status)
	{
		if (m_ResearchNode_Dict.ContainsKey(_id)) m_ResearchNode_Dict[_id].SetNodeStatus(_status);
	}
	public E_ResearchStatus GetResearchStatus(int _key)
	{
		if(!m_ResearchNode_Dict.ContainsKey(_key))
		{
			Debug.LogWarning("Research Node key not found");
			return E_ResearchStatus.Locked;
		}

		return m_ResearchNode_Dict[_key].GetNodeStatus();
	}
	public void ClosePanel() => m_ResearchCanvas.SetActive(false);
}
