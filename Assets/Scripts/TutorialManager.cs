using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager Instance;
	public static Action<string> s_Trigger;

	[SerializeField] private GameObject m_TutorialCanvas;
	[SerializeField] private GameObject m_CloseButton;
	[SerializeField] private GameObject m_NextPageButton;
	[SerializeField] private Tutorial[] m_TutorialHints;
	[SerializeField] private TMP_Text m_TutorialText;
	readonly Dictionary<string, Tutorial> m_TutorialDict = new();
	List<string> m_FinishedTutorialList = new();

	Tutorial m_CurrentTutorial;
	int m_CurrentMessageIdx = 0;

	private void OnEnable()
	{
		s_Trigger += Trigger;
	}
	private void OnDisable()
	{
		s_Trigger -= Trigger;
	}
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		Initialize();
	}
	void Initialize()
	{
		for (int i = 0; i < m_TutorialHints.Length; i++)
		{
			m_TutorialDict[m_TutorialHints[i].ID] = m_TutorialHints[i];
		}
		AssignFinishedTutorialIds();
		ClosePopup();
	}
	void AssignFinishedTutorialIds()
	{
		if (!PlayerPrefs.HasKey(Constant.PREF_TUTORIAL_FINISHED_IDS))
		{
			PlayerPrefs.SetString(Constant.PREF_TUTORIAL_FINISHED_IDS, "");
		}
		string _unlockedTutorial = PlayerPrefs.GetString(Constant.PREF_TUTORIAL_FINISHED_IDS, "");

		if (!string.IsNullOrEmpty(_unlockedTutorial))
		{
			string[] _unlocked = _unlockedTutorial.Split();

			foreach (string _id in _unlocked)
			{
				if (string.IsNullOrEmpty(_id)) continue;
				m_FinishedTutorialList.Add(_id);
			}
		}
	}
	public void Trigger(string _id)
	{
		Tutorial _hint = m_TutorialDict[_id];

		if (_hint == null) return;
		if (m_FinishedTutorialList.Contains(_id)) return;

		ShowPopup(_hint);
		m_FinishedTutorialList.Add(_id);

		string _tut = PlayerPrefs.GetString(Constant.PREF_TUTORIAL_FINISHED_IDS, "");
		PlayerPrefs.SetString(Constant.PREF_TUTORIAL_FINISHED_IDS, _tut + " " + _id);
	}

	void ShowPopup(Tutorial _hint)
	{
		m_TutorialCanvas.SetActive(true);
		m_CurrentTutorial = _hint;
		m_CurrentMessageIdx = 0;
		m_TutorialText.text = m_CurrentTutorial.Message[m_CurrentMessageIdx];
		CheckLastPage();
	}
	public void NextPage()
	{
		m_CurrentMessageIdx++;
		CheckLastPage();
		m_TutorialText.text = m_CurrentTutorial.Message[m_CurrentMessageIdx];
	}
	public void ClosePopup()
	{
		m_TutorialCanvas.SetActive(false);
	}

	void CheckLastPage()
	{
		bool _IsLastPage = m_CurrentMessageIdx == m_CurrentTutorial.Message.Length - 1;
		m_NextPageButton.SetActive(!_IsLastPage);
		m_CloseButton.SetActive(_IsLastPage);
	}
}