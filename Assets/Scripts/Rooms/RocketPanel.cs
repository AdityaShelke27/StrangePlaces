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

		m_RocketProgression = Mathf.Min(m_RocketProgression, 5);
		m_CurrentResourceRequirement = m_RocketResourceRequirement[m_RocketProgression];
		AddResources();
		CheckAvailableResources();
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

	}
	void AddResources()
	{
		if (m_CurrentResourceRequirement == null) return;

		m_RequiredResourcesTexts = new TMP_Text[m_CurrentResourceRequirement.Length];
		for (int i  = 0; i < m_CurrentResourceRequirement.Length; i++)
		{
			GameObject _objSlot = Instantiate(m_ResourceItemPrefab, m_ResourceInputParent);
			ResourceRequirementManager _objResourceManager = _objSlot.GetComponent<ResourceRequirementManager>();
			_objResourceManager.AssignResourceImageNameAndAmount(m_CurrentResourceRequirement[i].item.itemImage, m_CurrentResourceRequirement[i].item.itemName, m_CurrentResourceRequirement[i].amount.ToString());

			m_RequiredResourcesTexts[i] = _objResourceManager.GetAmountText();
		}
	}
	void CheckAvailableResources()
	{
		if(m_CurrentResourceRequirement == null) return;

		for (int i = 0; i < m_CurrentResourceRequirement.Length; i++)
		{
			m_RequiredResourcesTexts[i].color = ResourceTracker.Instance.SearchResourceAvailable(m_CurrentResourceRequirement[i].item as StorableItem, m_CurrentResourceRequirement[i].amount) ? Color.green : Color.red;
		}
	}
	void UpdateSpriteAndText()
	{
		m_BlueprintImage.sprite = m_RocketBlueprintSprites[m_RocketProgression];
		m_PartImage.sprite = m_RocketPartSprites[m_RocketProgression];
		m_RocketRenderer.sprite = m_RocketProgressionSprites[m_RocketProgression];
		m_PartText.text = m_RocketPartNames[m_RocketProgression];
	}

	public void ClosePanel() => m_RCPanelUI.SetActive(false);
}
