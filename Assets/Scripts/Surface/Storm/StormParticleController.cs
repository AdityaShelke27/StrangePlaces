using UnityEngine;

public class StormParticleController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private StormController m_StormController;

	[SerializeField] private ParticleSystem m_DustParticles;
	[SerializeField] private ParticleSystem m_WindStreakParticles;
	[SerializeField] private ParticleSystem m_DebrisParticles;
	[SerializeField] private ParticleSystem m_DustSheetParticles;

	[Header("Dust")]
	[SerializeField] private float m_MaxDustEmission = 80f;
	[SerializeField] private float m_MinDustSpeed = 0.5f;
	[SerializeField] private float m_MaxDustSpeed = 5f;

	[Header("Wind Streaks")]
	[SerializeField] private float m_MaxStreakEmission = 60f;
	[SerializeField] private float m_MinStreakSpeed = 4f;
	[SerializeField] private float m_MaxStreakSpeed = 15f;

	[Header("Debris")]
	[SerializeField] private float m_MinDebrisSpeed = 6f;
	[SerializeField] private float m_MaxDebrisSpeed = 18f;
	[SerializeField] private int m_MinDebrisBurst = 2;
	[SerializeField] private int m_MaxDebrisBurst = 8;

	[Header("Dust Sheets")]
	[SerializeField] private float m_MaxDustSheetEmission = 5f;
	[SerializeField] private float m_MinDustSheetSpeed = 1.5f;
	[SerializeField] private float m_MaxDustSheetSpeed = 6f;

	[SerializeField]
	private AnimationCurve m_DustSheetEmissionCurve = AnimationCurve.EaseInOut(0.4f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve m_DustEmissionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve m_StreakEmissionCurve = AnimationCurve.EaseInOut(0.2f, 0f, 1f, 1f);

	private ParticleSystem.EmissionModule m_DustEmission;
	private ParticleSystem.VelocityOverLifetimeModule m_DustVelocity;

	private ParticleSystem.EmissionModule m_StreakEmission;
	private ParticleSystem.VelocityOverLifetimeModule m_StreakVelocity;

	private ParticleSystem.VelocityOverLifetimeModule m_DebrisVelocity;
	private bool m_WasGusting;

	private ParticleSystem.EmissionModule m_DustSheetEmission;

	private ParticleSystem.VelocityOverLifetimeModule m_DustSheetVelocity;

	private void Awake()
	{
		m_DustEmission = m_DustParticles.emission;
		m_DustVelocity = m_DustParticles.velocityOverLifetime;

		m_StreakEmission = m_WindStreakParticles.emission;
		m_StreakVelocity = m_WindStreakParticles.velocityOverLifetime;

		m_DustVelocity.enabled = true;
		m_StreakVelocity.enabled = true;

		if (!m_DustParticles.isPlaying)
			m_DustParticles.Play();

		if (!m_WindStreakParticles.isPlaying)
			m_WindStreakParticles.Play();

		m_DebrisVelocity = m_DebrisParticles.velocityOverLifetime;
		m_DebrisVelocity.enabled = true;

		m_DustSheetEmission = m_DustSheetParticles.emission;
		m_DustSheetVelocity = m_DustSheetParticles.velocityOverLifetime;
		m_DustSheetVelocity.enabled = true;
	}

	private void Update()
	{
		float intensity = m_StormController.StormIntensity;

		UpdateDust(intensity);
		UpdateWindStreaks(intensity);
		UpdateDebris(m_StormController.StormIntensity, m_StormController.GustIntensity);
		UpdateDustSheets(m_StormController.StormIntensity, m_StormController.GustIntensity);
	}

	private void UpdateDust(float intensity)
	{
		float emission = m_DustEmissionCurve.Evaluate(intensity) * m_MaxDustEmission;

		m_DustEmission.rateOverTime = emission;

		float gust = m_StormController.GustIntensity;

		float speed = Mathf.Lerp(m_MinDustSpeed, m_MaxDustSpeed, intensity);

		speed *= Mathf.Lerp(1f, 1.8f, gust);

		Vector2 wind = m_StormController.WindDirection;

		m_DustVelocity.x = wind.x * speed;
		m_DustVelocity.y = wind.y * speed;
	}

	private void UpdateWindStreaks(float intensity)
	{
		float emission = m_StreakEmissionCurve.Evaluate(intensity) * m_MaxStreakEmission;

		float gustBoost = Mathf.Lerp(1f, 2.5f, m_StormController.GustIntensity);

		m_StreakEmission.rateOverTime = emission * gustBoost;

		float speed = Mathf.Lerp(m_MinStreakSpeed, m_MaxStreakSpeed, intensity);

		speed *= Mathf.Lerp(1f, 2f, m_StormController.GustIntensity);

		Vector2 wind = m_StormController.WindDirection;

		m_StreakVelocity.x = wind.x * speed;
		m_StreakVelocity.y = wind.y * speed;
	}
	private void UpdateDebris(float intensity, float gust)
	{
		float speed = Mathf.Lerp(m_MinDebrisSpeed, m_MaxDebrisSpeed, intensity);

		Vector2 wind = m_StormController.WindDirection;

		m_DebrisVelocity.x = wind.x * speed;
		m_DebrisVelocity.y = wind.y * speed;

		bool gustStarted = gust > 0.1f && !m_WasGusting;

		if (gustStarted && intensity > 0.4f)
		{
			int amount = Random.Range(m_MinDebrisBurst, m_MaxDebrisBurst + 1);
			m_DebrisParticles.Emit(amount);
		}

		m_WasGusting = gust > 0.1f;
	}
	private void UpdateDustSheets(float intensity, float gust)
	{
		float emission = m_DustSheetEmissionCurve.Evaluate(intensity) * m_MaxDustSheetEmission;
		m_DustSheetEmission.rateOverTime = emission;

		float speed = Mathf.Lerp(m_MinDustSheetSpeed, m_MaxDustSheetSpeed, intensity);
		speed *= Mathf.Lerp(1f, 1.4f, gust);

		Vector2 wind = m_StormController.WindDirection;

		m_DustSheetVelocity.x = wind.x * speed;
		m_DustSheetVelocity.y =wind.y * speed;
	}
}