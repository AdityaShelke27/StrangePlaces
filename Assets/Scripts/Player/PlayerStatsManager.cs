using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsManager : MonoBehaviour
{
	public static PlayerStatsManager Instance;
	[SerializeField] private int m_MaxElectricity;
	[SerializeField] private int m_MaxHunger;
	[SerializeField] private int m_Electricity;
	[SerializeField] private int m_Hunger;
	[SerializeField] private int m_ResearchPoints;
	int m_TotalHungerDepletionRate = 1;
	float m_HungerDepletionInterval = 3;
	[Header("UI")]
	[SerializeField] private Slider m_ElectricitySlider;
	[SerializeField] private Slider m_HungerSlider;
	[SerializeField] private TMP_Text m_ElectricityText;
	[SerializeField] private TMP_Text m_HungerText;
	[SerializeField] private TMP_Text m_ResearchPointsText;
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
	}
	private void Start()
	{
		m_ElectricitySlider.maxValue = m_MaxElectricity;
		m_HungerSlider.maxValue = m_MaxHunger;
		UpdateElectricityUI();
		UpdateHungerUI();
		UpdateResearchPointsUI();

		//StartCoroutine(DepleteHunger());
	}
	IEnumerator DepleteHunger()
	{
		while(true)
		{
			if (m_Hunger <= 0)
			{
				Debug.LogWarning("Player severly hungry, died of hunger");
				yield break;
			}

			AddHunger(-m_TotalHungerDepletionRate);

			yield return new WaitForSeconds(m_HungerDepletionInterval);
		}
	}
	public void SetElectricity(int _electricity)
	{
		m_Electricity = _electricity;
		UpdateElectricityUI();
	}
	public void SetHunger(int _hunger)
	{
		m_Hunger = _hunger;
		UpdateHungerUI();
	}
	public void SetResearchPoints(int _researchPoints)
	{
		m_ResearchPoints = _researchPoints;
		UpdateResearchPointsUI();
	}
	public void AddElectricity(int _val)
	{
		int _sum = m_Electricity + _val;
		if(_sum < 0)
		{
			Debug.LogWarning($"The added electricity value makes the total electricity less than 0, AMOUNT: {_val}");
			return;
		}
		else if(_sum > m_MaxElectricity)
		{
			_sum = m_MaxElectricity;
		}
		m_Electricity = _sum;
		UpdateElectricityUI();
	}
	public void AddHunger(int _val)
	{
		int _sum = m_Hunger + _val;
		if (_sum < 0)
		{
			Debug.LogWarning($"The added hunger value makes the total hunger less than 0, AMOUNT: {_val}");
			return;
		}
		else if (_sum > m_MaxHunger)
		{
			_sum = m_MaxHunger;
		}
		m_Hunger = _sum;
		UpdateHungerUI();
	}
	public void AddResearchPoints(int _val)
	{
		int _sum = m_ResearchPoints + _val;
		if (_sum < 0)
		{
			Debug.LogWarning($"The added research points value makes the total research points less than 0, AMOUNT: {_val}");
			return;
		}

		m_ResearchPoints = _sum;
		UpdateResearchPointsUI();
	}
	private void UpdateElectricityUI()
	{
		m_ElectricitySlider.value = m_Electricity;
		m_ElectricityText.text = m_Electricity.ToString();
	}
	private void UpdateHungerUI()
	{
		m_HungerSlider.value = m_Hunger;
		m_HungerText.text = m_Hunger.ToString();
	}
	private void UpdateResearchPointsUI()
	{
		m_ResearchPointsText.text = m_ResearchPoints.ToString();
	}
}
