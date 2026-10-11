using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial", menuName = "Scriptable Objects/Tutorial")]
public class Tutorial : ScriptableObject
{
	public string ID;
	[TextArea]
	public string[] Message;
}
