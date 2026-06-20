using UnityEngine;
using UnityEngine.EventSystems;

public class Construction : MonoBehaviour
{
	[SerializeField] GameObject m_MachinePanelUI;
	[SerializeField] GameObject m_MachineCraftButtonPrefab;
	[SerializeField] GameObject m_ResourceRequirementPrefab;
	[SerializeField] Transform m_ButtonContentParent;
	[SerializeField] CraftObject[] m_CraftMachines;

	private void Start()
	{
		ClosePanel();
		AssignMachineCraftingData();
	}
	private void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		m_MachinePanelUI.SetActive(true);
	}
	private void AssignMachineCraftingData()
	{
		for(int i = 0; i < m_CraftMachines.Length; i++)
		{
			GameObject _objButton = Instantiate(m_MachineCraftButtonPrefab, m_ButtonContentParent);
			MachineCraftingButtonManager _buttonManager = _objButton.GetComponent<MachineCraftingButtonManager>();
			_buttonManager.SetName(m_CraftMachines[i].CraftItem.itemName);
			_buttonManager.SetImage(m_CraftMachines[i].CraftItem.itemImage);

			ResourceRequirement[] _requirements = m_CraftMachines[i].ResourceRequirements;
			for (int j = 0; j < _requirements.Length; j++)
			{
				ResourceRequirement _requirement = _requirements[j];
				GameObject _objSlot = Instantiate(m_ResourceRequirementPrefab, _buttonManager.GetResourceInputParent());
				_objSlot.GetComponent<ResourceRequirementManager>().AssignResourceImageAndAmount(_requirement.item.itemImage, _requirement.amount.ToString());
			}
		}
	}
	public void ClosePanel() => m_MachinePanelUI.SetActive(false);
}
