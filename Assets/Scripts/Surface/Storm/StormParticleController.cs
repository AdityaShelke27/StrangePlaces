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

	[SerializeField] private int m_MinDebrisBurst = 4;
	[SerializeField] private int m_MaxDebrisBurst = 12;

	[SerializeField] private float m_MinDebrisInterval = 0.1f;
	[SerializeField] private float m_MaxDebrisInterval = 0.35f;

	private float m_DebrisTimer;
	private float m_NextDebrisBurst;

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
		ScheduleNextDebrisBurst();

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
		Vector2 wind = m_StormController.WindDirection;

		float speed = Mathf.Lerp(m_MinDebrisSpeed, m_MaxDebrisSpeed, intensity);
		speed *= Mathf.Lerp( 1f, 1.5f, gust);

		m_DebrisVelocity.x = wind.x * speed;
		m_DebrisVelocity.y = wind.y * speed;

		// No debris during weaker storm stages.
		if (intensity < 0.4f)
		{
			m_DebrisTimer = 0f;
			return;
		}

		// Debris becomes much more active during gusts.
		float activity = Mathf.Lerp(0.25f, 1f, gust);

		m_DebrisTimer += Time.deltaTime * activity;

		if (m_DebrisTimer >= m_NextDebrisBurst)
		{
			EmitDebris(intensity, gust);
			ScheduleNextDebrisBurst();
		}
	}
	private void EmitDebris(float intensity, float gust)
	{
		float amountFactor = Mathf.Clamp01(intensity * 0.7f + gust * 0.6f);

		int maxAmount = Mathf.RoundToInt(Mathf.Lerp(m_MinDebrisBurst, m_MaxDebrisBurst, amountFactor));
		int amount = Random.Range(m_MinDebrisBurst, maxAmount + 1);

		m_DebrisParticles.Emit(amount);
	}
	private void ScheduleNextDebrisBurst()
	{
		m_DebrisTimer = 0f;
		m_NextDebrisBurst = Random.Range(m_MinDebrisInterval, m_MaxDebrisInterval);
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