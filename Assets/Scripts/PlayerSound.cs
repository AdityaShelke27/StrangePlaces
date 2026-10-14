using UnityEngine;

public class PlayerSound : MonoBehaviour
{
	[SerializeField] private AudioSource m_AudioSource;
	[SerializeField] private float m_MinPitch;
	[SerializeField] private float m_MaxPitch;
	[SerializeField] private AudioClip m_FootstepClip;
	[SerializeField] private AudioClip m_LadderClimbClip;
	[SerializeField] private AudioClip m_ConstructClimbClip;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PlayFootstep()
	{
		m_AudioSource.clip = m_FootstepClip;
		m_AudioSource.pitch = Random.Range(m_MinPitch, m_MaxPitch);
		m_AudioSource.Play();
	}
	public void PlayLadderClimb()
	{
		m_AudioSource.clip = m_LadderClimbClip;
		m_AudioSource.pitch = Random.Range(m_MinPitch, m_MaxPitch);
		m_AudioSource.Play();
	}
	public void PlayConstructClip()
	{
		m_AudioSource.clip = m_ConstructClimbClip;
		m_AudioSource.pitch = Random.Range(m_MinPitch, m_MaxPitch);
		m_AudioSource.Play();
	}
}
