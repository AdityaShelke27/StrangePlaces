using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
	public static ItemDatabase Instance;

	[SerializeField] Item[] m_Items;
	Dictionary<string, Item> m_ItemDict;

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
	}
}
