using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
	public static SurfaceManager Instance;
	public static Action<ResourceNodeInstance> s_ShowNodeDetails;

	[SerializeField] private GameObject m_TutorialManager;
	[SerializeField] private GameObject m_DetailPanel;
	[SerializeField] private TMP_Text m_NameText;
	[SerializeField] private GameObject m_ObjectivePanel;
	bool m_IsShowingPanel = false;

	private void OnEnable()
	{
		s_ShowNodeDetails += SetNodeDetails;
	}
	private void OnDisable()
	{
		s_ShowNodeDetails -= SetNodeDetails;
	}
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		if (PlayerPrefs.GetInt(Constant.PREF_TUTORIAL) == 1)
		{
			Instantiate(m_TutorialManager);
		}
	}
	private void Start()
	{
		m_DetailPanel.SetActive(false);
		CloseObjectivePanel();

		StartCoroutine(StartMachineTutorial());
	}
	IEnumerator StartMachineTutorial()
	{
		yield return new WaitForSeconds(1);

		InventorySlot[] _inv = ResourceTracker.Instance.GetPlayerInventory();

		foreach (InventorySlot slot in _inv)
		{
			if(slot.GetItem() is Machine)
			{
				TutorialManager.s_Trigger?.Invoke("6");
				yield break;
			}
		}
	}
	void SetNodeDetails(ResourceNodeInstance _node)
	{
		if (m_IsShowingPanel) return;

		StartCoroutine(ShowNode(_node));
	}
	IEnumerator ShowNode(ResourceNodeInstance _node)
	{
		m_DetailPanel.transform.position = _node.gameObject.transform.position + Vector3.up;
		ResourceNode _data = _node.GetResourceNodeData();
		m_NameText.text = ItemDatabase.Instance.DoesItemIDExistInResearch(_data.itemID) ? _data.itemName : "???";
		m_DetailPanel.SetActive(true);
		m_IsShowingPanel = true;

		yield return new WaitForSeconds(2);

		m_DetailPanel.SetActive(false);
		m_IsShowingPanel = false;
	}
	public void ShowObjectivePanel()
	{
		m_ObjectivePanel.SetActive(true);
	}

	public void CloseObjectivePanel()
	{
		m_ObjectivePanel.SetActive(false);
	}
}
