using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RocketPanel : MonoBehaviour
{
	public static RocketPanel Instance;
	[SerializeField] GameObject m_RCPanelUI;
	[SerializeField] int m_RocketProgression;
	[SerializeField] Image m_BlueprintImage;
	[SerializeField] Image m_PartImage;
	[SerializeField] TMP_Text m_PartText;
	[SerializeField] SpriteRenderer m_RocketRenderer;
	[SerializeField] Transform m_ResourceInputParent;
	[SerializeField] GameObject m_ResourceItemPrefab;
	[SerializeField] Button m_ConstructButton;
	[SerializeField] TMP_Text m_ConstructButtonText;
	[SerializeField] GameObject[] m_SizePanelProgressDone;
	[Header("Resource Requirement")]
	[SerializeField] ResourceRequirement[] m_RocketFrameRequirement;
	[SerializeField] ResourceRequirement[] m_QuantumProcessorRequirement;
	[SerializeField] ResourceRequirement[] m_EngineRequirement;
	[SerializeField] ResourceRequirement[] m_NavigationModuleRequirement;
	[SerializeField] ResourceRequirement[] m_FuelTankRequirement;
	[SerializeField] ResourceRequirement[] m_CockpitRequirement;
	[Header("Data")]
	[SerializeField] Sprite[] m_RocketPartSprites;
	[SerializeField] Sprite[] m_RocketProgressionSprites;
	[SerializeField] Sprite[] m_RocketBlueprintSprites;
	[SerializeField] string[] m_RocketPartNames;

	TMP_Text[] m_RequiredResourcesTexts;
	ResourceRequirement[][] m_RocketResourceRequirement;
	ResourceRequirement[] m_CurrentResourceRequirement;

	int[] m_CurrentResourcesAdded;

	bool m_IsResearchComplete = false;
	bool m_IsInConstructMode = false;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}
	private void Start()
	{
		ClosePanel();

		m_RocketResourceRequirement = new ResourceRequirement[][]
		{
			m_RocketFrameRequirement,
			m_QuantumProcessorRequirement,
			m_EngineRequirement,
			m_NavigationModuleRequirement,
			m_FuelTankRequirement,
			m_CockpitRequirement,
		};

		m_RocketProgression = Mathf.Min(m_RocketProgression, 6);
		m_IsResearchComplete = !(m_RocketProgression <= 5);
		m_ConstructButton.gameObject.SetActive(!m_IsResearchComplete);

		for(int i = 0; i < m_RocketProgression; i++)
		{
			m_SizePanelProgressDone[i].SetActive(true);
		}

		m_CurrentResourceRequirement = m_RocketResourceRequirement[m_RocketProgression];
		m_CurrentResourcesAdded = new int[m_CurrentResourceRequirement.Length];

		AddResources();
		UpdateConstructMode();
		UpdateSpriteAndText();
	}
	private void OnMouseDown()
	{
		StartCoroutine(Constant.DelayExecute(() =>
		{
			if (EventSystem.current.IsPointerOverGameObject()) return;

			m_RCPanelUI.SetActive(true);
		}));
	}
	public void ConstructButton()
	{
		if (m_CurrentResourceRequirement == null) return;

		if(m_IsInConstructMode)
		{
			m_SizePanelProgressDone[m_RocketProgression].SetActive(true);
			m_RocketProgression++;
			m_IsResearchComplete = !(m_RocketProgression <= 5);
			m_ConstructButton.gameObject.SetActive(!m_IsResearchComplete);

			if (!m_IsResearchComplete) 
			{ 
				m_CurrentResourceRequirement = m_RocketResourceRequirement[m_RocketProgression];
				m_CurrentResourcesAdded = new int[m_CurrentResourceRequirement.Length];
			}
			UpdateSpriteAndText();
			AddResources();

			m_IsInConstructMode = false;
		}
		else
		{
			ResourceRequirement[] _requirements = m_CurrentResourceRequirement;
			for (int j = 0; j < _requirements.Length; j++)
			{
				int _fetchedAmount = ResourceTracker.Instance.SearchAndRemoveMaxAmountResource(_requirements[j].item as StorableItem, _requirements[j].amount - m_CurrentResourcesAdded[j]);
				m_CurrentResourcesAdded[j] += _fetchedAmount;
			}

			UpdateConstructMode();
		}
	}
	void UpdateConstructMode()
	{
		if(m_CurrentResourceRequirement.Length != m_CurrentResourcesAdded.Length)
		{
			Debug.LogWarning($"Current Resource Requirement length and Current Resource Added length are not equal: Current Resource Requirement = {m_CurrentResourceRequirement.Length}, Current Resource Added = {m_CurrentResourcesAdded.Length}");
		}

		bool _areAllResourceAdded = true;
		for(int i = 0; i < m_CurrentResourceRequirement.Length; i++)
		{
			if (m_CurrentResourcesAdded[i] < m_CurrentResourceRequirement[i].amount)
			{
				_areAllResourceAdded = false;
				break;
			}
		}
		CheckAvailableResources();
		m_IsInConstructMode = _areAllResourceAdded;

		m_ConstructButtonText.text = m_IsInConstructMode ? "Construct" : "Add Resources";
	}
	void AddResources()
	{
		if (m_CurrentResourceRequirement == null) return;

		for(int i = 0; i < m_ResourceInputParent.childCount; i++)
		{
			Destroy(m_ResourceInputParent.GetChild(i).gameObject);
		}

		m_RequiredResourcesTexts = new TMP_Text[m_CurrentResourceRequirement.Length];
		for (int i  = 0; i < m_CurrentResourceRequirement.Length; i++)
		{
			GameObject _objSlot = Instantiate(m_ResourceItemPrefab, m_ResourceInputParent);
			ResourceRequirementManager _objResourceManager = _objSlot.GetComponent<ResourceRequirementManager>();
			_objResourceManager.AssignResourceImageNameAndAmount(m_CurrentResourceRequirement[i].item.itemImage, m_CurrentResourceRequirement[i].item.itemName, m_CurrentResourceRequirement[i].amount.ToString());

			m_RequiredResourcesTexts[i] = _objResourceManager.GetAmountText();
		}

		CheckAvailableResources();
	}
	void CheckAvailableResources()
	{
		if(m_CurrentResourceRequirement == null) return;

		for (int i = 0; i < m_CurrentResourceRequirement.Length; i++)
		{
			m_RequiredResourcesTexts[i].text = $"{m_CurrentResourcesAdded[i]} / {m_CurrentResourceRequirement[i].amount}";
			m_RequiredResourcesTexts[i].color = m_CurrentResourcesAdded[i] == m_CurrentResourceRequirement[i].amount ? Color.green : Color.red;
		}
	}
	void UpdateSpriteAndText()
	{
		int _rocketProgress = Mathf.Min(m_RocketProgression, 5);

		m_BlueprintImage.sprite = m_RocketBlueprintSprites[_rocketProgress];
		m_PartImage.sprite = m_RocketPartSprites[_rocketProgress];
		m_RocketRenderer.sprite = m_RocketProgressionSprites[_rocketProgress];
		m_PartText.text = m_RocketPartNames[_rocketProgress];

		m_BlueprintImage.preserveAspect = true;
		m_PartImage.preserveAspect = true;
	}

	public void ClosePanel() => m_RCPanelUI.SetActive(false);
}
