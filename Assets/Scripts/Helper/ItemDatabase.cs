using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
	public static ItemDatabase Instance;

	[SerializeField] Item[] m_Items;
	readonly Dictionary<string, Item> m_ItemDict = new();
	List<int> m_UnlockedResearchIDs = new();

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else if(Instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		Initialize();
	}
	void Initialize()
	{
		for(int i = 0; i < m_Items.Length; i++)
		{
			m_ItemDict[m_Items[i].itemID] = m_Items[i];
		}

		AssignResearchIds();
	}
	public Item GetItemByID(string _itemID)
	{
		return m_ItemDict.ContainsKey(_itemID) ? m_ItemDict[_itemID] : null;
	}
	public Item[] GetAllItems() => m_Items;

	void AssignResearchIds()
	{
		string _unlockedResearchStr = PlayerPrefs.GetString(Constant.PREF_RESEARCHEDNODES, "0");

		if (!string.IsNullOrEmpty(_unlockedResearchStr))
		{
			string[] _unlocked = _unlockedResearchStr.Split();

			foreach (string _id in _unlocked)
			{
				if (string.IsNullOrEmpty(_id)) continue;

				int _numID = Convert.ToInt32(_id);

				m_UnlockedResearchIDs.Add(_numID);
			}
		}
	}

	public bool DoesItemIDExistInResearch(string _id)
	{
		foreach (int _key in m_UnlockedResearchIDs)
		{
			if (!Constant.m_ResearchID_ToItemID.ContainsKey(_key)) continue;
			if (Constant.m_ResearchID_ToItemID[_key].Contains(_id)) return true;
		}

		return false;
	}
}
