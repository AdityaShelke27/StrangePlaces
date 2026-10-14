using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	private AudioSource m_AudioSource;
	[SerializeField] private AudioClip m_ButtonClickClip;
	[SerializeField] private AudioClip m_ButtonErrorClip;
	[SerializeField] private AudioClip m_RoomConstructClip;
	private void Awake()
	{
		if(Instance == null) Instance = this;

		m_AudioSource = GetComponent<AudioSource>();
	}

	public void PlayButtonClick()
	{
		m_AudioSource.clip = m_ButtonClickClip;
		m_AudioSource.Play();
	}
	public void PlayButtonError()
	{
		m_AudioSource.clip = m_ButtonErrorClip;
		m_AudioSource.Play();
	}
	public void PlayRoomConstruct()
	{
		m_AudioSource.clip = m_RoomConstructClip;
		m_AudioSource.Play();
	}
}
