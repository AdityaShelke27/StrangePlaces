using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRequirementManager : MonoBehaviour
{
	[SerializeField] Image m_Image;
	[SerializeField] TMP_Text m_Amount;

	public void AssignResourceImageAndAmount(Sprite _image, string _text)
	{
		m_Image.sprite = _image;
		m_Amount.text = _text;
	}
}
