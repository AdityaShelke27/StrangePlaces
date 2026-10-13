using UnityEngine;

public class StormAudioController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private StormController m_StormController;

	[Header("Wind Sources")]
	[SerializeField] private AudioSource m_LightWind;
	[SerializeField] private AudioSource m_StrongWind;
	[SerializeField] private AudioSource m_HeavyStorm;

	[Header("Maximum Volumes")]
	[SerializeField, Range(0f, 1f)]
	private float m_LightWindMaxVolume = 0.45f;

	[SerializeField, Range(0f, 1f)]
	private float m_StrongWindMaxVolume = 0.65f;

	[SerializeField, Range(0f, 1f)]
	private float m_HeavyStormMaxVolume = 0.8f;

	[Header("Volume Response")]
	[SerializeField] private float m_VolumeChangeSpeed = 2f;

	[Header("Gust")]
	[SerializeField, Range(0f, 1f)]
	private float m_GustVolumeBoost = 0.15f;

	[Header("Gust Audio")]
	[SerializeField] private AudioSource m_GustSource;

	[SerializeField] private AudioClip[] m_GustClips;

	[SerializeField, Range(0f, 1f)]
	private float m_MinGustVolume = 0.25f;

	[SerializeField, Range(0f, 1f)]
	private float m_MaxGustVolume = 0.75f;

	[SerializeField]
	private Vector2 m_GustPitchRange =
		new Vector2(0.9f, 1.1f);

	[Header("Debris Audio")]
	[SerializeField] private StormParticleController m_ParticleController;

	[SerializeField] private AudioSource m_DebrisSource;

	[SerializeField] private AudioClip[] m_DebrisClips;

	[SerializeField, Range(0f, 1f)]
	private float m_MinDebrisVolume = 0.1f;

	[SerializeField, Range(0f, 1f)]
	private float m_MaxDebrisVolume = 0.4f;

	[SerializeField]
	private Vector2 m_DebrisPitchRange =
		new Vector2(0.9f, 1.15f);

	private bool m_WasGusting;

	private void OnEnable()
	{
		if (m_ParticleController != null)
			m_ParticleController.OnDebrisBurst += PlayDebris;
	}

	private void OnDisable()
	{
		if (m_ParticleController != null)
			m_ParticleController.OnDebrisBurst -= PlayDebris;
	}

	private void Awake()
	{
		StartSource(m_LightWind);
		StartSource(m_StrongWind);
		StartSource(m_HeavyStorm);
	}

	private void Update()
	{
		UpdateWindAudio();
		UpdateGustAudio();
	}

	private void UpdateWindAudio()
	{
		float intensity =
			m_StormController.StormIntensity;

		float gust =
			m_StormController.GustIntensity;

		float lightTarget =
			CalculateLightWind(intensity);

		float strongTarget =
			CalculateStrongWind(intensity);

		float heavyTarget =
			CalculateHeavyStorm(intensity);

		// Gusts briefly increase the stronger wind layers.
		strongTarget +=
			gust * m_GustVolumeBoost;

		heavyTarget +=
			gust * m_GustVolumeBoost;

		SetVolume(
			m_LightWind,
			lightTarget
		);

		SetVolume(
			m_StrongWind,
			strongTarget
		);

		SetVolume(
			m_HeavyStorm,
			heavyTarget
		);
	}

	private float CalculateLightWind(float intensity)
	{
		// Fades in early, then disappears as the storm becomes heavy.
		float fadeIn =
			Mathf.InverseLerp(
				0f,
				0.25f,
				intensity
			);

		float fadeOut =
			1f - Mathf.InverseLerp(
				0.35f,
				0.65f,
				intensity
			);

		return
			fadeIn *
			fadeOut *
			m_LightWindMaxVolume;
	}

	private float CalculateStrongWind(float intensity)
	{
		// Dominates the middle of the storm buildup.
		float fadeIn =
			Mathf.InverseLerp(
				0.2f,
				0.55f,
				intensity
			);

		float fadeOut =
			1f - Mathf.InverseLerp(
				0.75f,
				1f,
				intensity
			);

		return
			fadeIn *
			fadeOut *
			m_StrongWindMaxVolume;
	}

	private float CalculateHeavyStorm(float intensity)
	{
		// Starts late and dominates full storm.
		float fadeIn =
			Mathf.InverseLerp(
				0.55f,
				1f,
				intensity
			);

		return
			fadeIn *
			m_HeavyStormMaxVolume;
	}

	private void SetVolume(
		AudioSource source,
		float targetVolume)
	{
		targetVolume =
			Mathf.Clamp01(targetVolume);

		source.volume =
			Mathf.MoveTowards(
				source.volume,
				targetVolume,
				m_VolumeChangeSpeed * Time.deltaTime
			);
	}

	private void StartSource(AudioSource source)
	{
		if (source == null)
			return;

		source.loop = true;

		if (!source.isPlaying)
			source.Play();

		source.volume = 0f;
	}
	private void UpdateGustAudio()
	{
		float gust =
			m_StormController.GustIntensity;

		bool isGusting =
			gust > 0.05f;

		if (isGusting && !m_WasGusting)
		{
			PlayGust();
		}

		m_WasGusting = isGusting;
	}
	private void PlayGust()
	{
		if (m_GustClips == null ||
			m_GustClips.Length == 0)
			return;

		AudioClip clip =
			m_GustClips[
				Random.Range(
					0,
					m_GustClips.Length
				)
			];

		float stormIntensity =
			m_StormController.StormIntensity;

		float volume =
			Mathf.Lerp(
				m_MinGustVolume,
				m_MaxGustVolume,
				stormIntensity
			);

		m_GustSource.pitch =
			Random.Range(
				m_GustPitchRange.x,
				m_GustPitchRange.y
			);

		m_GustSource.PlayOneShot(
			clip,
			volume
		);
	}
	private void PlayDebris(float intensity)
	{
		if (m_DebrisClips == null ||
			m_DebrisClips.Length == 0)
			return;

		AudioClip clip =
			m_DebrisClips[
				Random.Range(
					0,
					m_DebrisClips.Length
				)
			];

		float volume =
			Mathf.Lerp(
				m_MinDebrisVolume,
				m_MaxDebrisVolume,
				intensity
			);

		m_DebrisSource.pitch =
			Random.Range(
				m_DebrisPitchRange.x,
				m_DebrisPitchRange.y
			);

		m_DebrisSource.PlayOneShot(
			clip,
			volume
		);
	}
}