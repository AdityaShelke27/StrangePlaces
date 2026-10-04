using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRequirementManager : MonoBehaviour
{
	[SerializeField] Image m_Image;
	[SerializeField] TMP_Text m_Title;
	[SerializeField] TMP_Text m_Amount;

	public void AssignResourceImageNameAndAmount(Sprite _image, string _name, string _amount)
	{
		m_Image.sprite = _image;
		m_Title.text = _name;
		m_Amount.text = _amount;
	}

	public void SetResourceAvailableStatus(bool _areResourceAvailable)
	{
		if(_areResourceAvailable)
		{
			m_Title.color = Constant.COLOR_RESOURCE_AVAILABLE;
			m_Amount.color = Constant.COLOR_RESOURCE_AVAILABLE;
		}
		else
		{
			m_Title.color = Constant.COLOR_RESOURCE_UNAVAILABLE;
			m_Amount.color = Constant.COLOR_RESOURCE_UNAVAILABLE;
		}
	}
	public TMP_Text GetAmountText() => m_Amount;
}
