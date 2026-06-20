using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineCraftingButtonManager : MonoBehaviour
{
	[SerializeField] Image m_Image;
	[SerializeField] TMP_Text m_MachineName;
	[SerializeField] Button m_Button;
	[SerializeField] Transform m_ResourceInputParent;

	public void SetName(string _name) => m_MachineName.text = _name;
	public void SetImage(Sprite _image) => m_Image.sprite = _image;
	public Button GetButton() => m_Button;
	public Transform GetResourceInputParent() => m_ResourceInputParent;
}
