using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomConstruction : MonoBehaviour
{
	[SerializeField] GameObject RoomConstructionPanel;
	[SerializeField] GameObject m_ResourceItemPrefab;
	[SerializeField] Transform[] m_InputList;
	[SerializeField] ResourceRequirement[] m_MechanicRoomResourceRequirement;
	[SerializeField] ResourceRequirement[] m_ConstructionRoomResourceRequirement;
	[SerializeField] ResourceRequirement[] m_StorageRoomResourceRequirement;
	[SerializeField] ResourceRequirement[] m_ResearchRoomResourceRequirement;
	[SerializeField] ResourceRequirement[] m_KitchenRoomResourceRequirement;
	[SerializeField] ResourceRequirement[] m_RocketConstructionRoomResourceRequirement;
	[SerializeField] GameObject[] m_NotResearchedPanel;
	bool m_IsRoomConstructionPanelActive = false;

	ResourceRequirement[][] m_RoomCostRequirement;
	TMP_Text[][] m_RequiredResourcesTexts;

	private void OnEnable()
	{
		ResearchNode.s_ResearchedAction += UnlockRoomResearched;
	}
	private void OnDisable()
	{
		ResearchNode.s_ResearchedAction -= UnlockRoomResearched;
	}
	void Start()
	{
		m_RoomCostRequirement = new ResourceRequirement[][]
		{
			m_MechanicRoomResourceRequirement,
			m_ConstructionRoomResourceRequirement,
			m_StorageRoomResourceRequirement,
			m_ResearchRoomResourceRequirement,
			m_KitchenRoomResourceRequirement,
			m_RocketConstructionRoomResourceRequirement
		};

		RoomConstructionPanel.SetActive(m_IsRoomConstructionPanelActive);

		for (int i = 0; i < m_NotResearchedPanel.Length; i++)
		{
			bool _isResearched = ItemDatabase.Instance.DoesItemIDExistInResearch(i.ToString());

			if (m_NotResearchedPanel[i] == null) continue;

			m_NotResearchedPanel[i].SetActive(!_isResearched);
			m_NotResearchedPanel[i].transform.parent.GetComponent<Button>().interactable = _isResearched;
		}

		AssignCraftingData();
	}
	void AssignCraftingData()
	{
		m_RequiredResourcesTexts = new TMP_Text[m_InputList.Length][];

		for(int i = 0; i < m_InputList.Length; i++)
		{
			m_RequiredResourcesTexts[i] = new TMP_Text[m_RoomCostRequirement[i].Length];

			for(int j = 0; j < m_RoomCostRequirement[i].Length; j++)
			{
				GameObject _objSlot = Instantiate(m_ResourceItemPrefab, m_InputList[i]);
				ResourceRequirementManager _objResourceManager = _objSlot.GetComponent<ResourceRequirementManager>();
				_objResourceManager.AssignResourceImageNameAndAmount(m_RoomCostRequirement[i][j].item.itemImage, m_RoomCostRequirement[i][j].item.itemName, m_RoomCostRequirement[i][j].amount.ToString());

				m_RequiredResourcesTexts[i][j] = _objResourceManager.GetAmountText();
			}
		}
	}
	void CheckAvailableResources()
	{
		for (int i = 0; i < m_RoomCostRequirement.Length; i++)
		{
			ResourceRequirement[] _requirements = m_RoomCostRequirement[i];
			for (int j = 0; j < _requirements.Length; j++)
			{
				ResourceRequirement _requirement = _requirements[j];
				m_RequiredResourcesTexts[i][j].color = ResourceTracker.Instance.SearchResourceAvailable(_requirement.item as StorableItem, _requirement.amount) ? Color.green : Color.red;
			}
		}
	}
	bool CheckBuildResources(ResourceRequirement[] _requirements)
	{
		bool _areResourcesAvailable = true;
		for (int j = 0; j < _requirements.Length; j++)
		{
			if (!ResourceTracker.Instance.SearchResourceAvailable(_requirements[j].item as StorableItem, _requirements[j].amount))
			{
				_areResourcesAvailable = false;
				break;
			}
		}
		if (!_areResourcesAvailable)
		{
			Debug.LogWarning("Not enough resources available in the inventory");
			return false;
		}
		
		CheckAvailableResources();

		return true;
	}
	void ConsumeBuildResources(ResourceRequirement[] _requirements)
	{
		for (int j = 0; j < _requirements.Length; j++)
		{
			ResourceTracker.Instance.SearchAndRemoveResource(_requirements[j].item as StorableItem, _requirements[j].amount);
		}
	}
	public void ToggleRoomConstructionPanel()
	{
		CheckAvailableResources();
		m_IsRoomConstructionPanelActive = !m_IsRoomConstructionPanelActive;
		RoomConstructionPanel.SetActive(m_IsRoomConstructionPanelActive);
	}
	void UnlockRoomResearched(int _id)
	{
		for (int i = 0; i < m_NotResearchedPanel.Length; i++)
		{
			bool _isResearched = ItemDatabase.Instance.DoesItemIDExistInResearch(i.ToString());

			if (m_NotResearchedPanel[i] == null) continue;

			m_NotResearchedPanel[i].SetActive(!_isResearched);
			m_NotResearchedPanel[i].transform.parent.GetComponent<Button>().interactable = _isResearched;
		}
	}
	public void GenerateConstructionRoom()
	{
		if (!CheckBuildResources(m_ConstructionRoomResourceRequirement)) return;

		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_CONSTRUCTION_ROOM, () => ConsumeBuildResources(m_ConstructionRoomResourceRequirement));
	}
	public void GenerateMechanicRoom()
	{
		if (!CheckBuildResources(m_MechanicRoomResourceRequirement)) return;

		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_MECHANIC_ROOM, () => ConsumeBuildResources(m_MechanicRoomResourceRequirement));
	}
	public void GenerateStorageRoom()
	{
		if (!CheckBuildResources(m_StorageRoomResourceRequirement)) return;

		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_STORAGE_ROOM, () => ConsumeBuildResources(m_StorageRoomResourceRequirement));
	}
	public void GenerateKitchen()
	{
		if (!CheckBuildResources(m_KitchenRoomResourceRequirement)) return;

		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_KITCHEN_ROOM, () => ConsumeBuildResources(m_KitchenRoomResourceRequirement));
	}
	public void GenerateResearchRoom()
	{
		if (!CheckBuildResources(m_ResearchRoomResourceRequirement)) return;

		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROOM, Constant.ID_RESEARCH_ROOM, () => ConsumeBuildResources(m_ResearchRoomResourceRequirement));
	}
	public void GenerateRocketConstructionRoom()
	{
		if (!CheckBuildResources(m_RocketConstructionRoomResourceRequirement)) return;

		ToggleRoomConstructionPanel();
		RoomPlacement.s_GenerateAreas(Constant.SIZE_ROCKET_ROOM, Constant.ID_ROCKETCONSTRUCTION_ROOM, () => ConsumeBuildResources(m_RocketConstructionRoomResourceRequirement));
	}
}
