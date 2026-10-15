using Solo.MOST_IN_ONE;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	private AudioSource m_AudioSource;
	[SerializeField] private AudioClip m_ButtonClickClip;
	[SerializeField] private AudioClip m_ButtonErrorClip;
	[SerializeField] private AudioClip m_RoomConstructClip;
	[SerializeField] private AudioClip m_BunkerOpenClip;
	private void Awake()
	{
		if(Instance == null) Instance = this;

		m_AudioSource = GetComponent<AudioSource>();
	}

	public void PlayButtonClick()
	{
		m_AudioSource.clip = m_ButtonClickClip;
		m_AudioSource.volume = 0.3f;
		m_AudioSource.Play();
		MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.SoftImpact);
	}
	public void PlayButtonError()
	{
		m_AudioSource.clip = m_ButtonErrorClip;
		m_AudioSource.volume = 0.3f;
		m_AudioSource.Play();
		MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Failure);
	}
	public void PlayRoomConstruct()
	{
		m_AudioSource.clip = m_RoomConstructClip;
		m_AudioSource.volume = 0.3f;
		m_AudioSource.Play();
		MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Success);
	}

	public void PlayBunkerOpen()
	{
		m_AudioSource.clip = m_BunkerOpenClip;
		m_AudioSource.volume = 0.7f;
		m_AudioSource.Play();
	}
}
