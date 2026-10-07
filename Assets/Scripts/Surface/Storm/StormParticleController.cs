using UnityEngine;

public class StormParticleController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private StormController m_StormController;

	[SerializeField] private ParticleSystem m_DustParticles;
	[SerializeField] private ParticleSystem m_WindStreakParticles;

	[Header("Dust")]
	[SerializeField] private float m_MaxDustEmission = 80f;
	[SerializeField] private float m_MinDustSpeed = 0.5f;
	[SerializeField] private float m_MaxDustSpeed = 5f;

	[Header("Wind Streaks")]
	[SerializeField] private float m_MaxStreakEmission = 60f;
	[SerializeField] private float m_MinStreakSpeed = 4f;
	[SerializeField] private float m_MaxStreakSpeed = 15f;

	[SerializeField]
	private AnimationCurve m_DustEmissionCurve =
		AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve m_StreakEmissionCurve =
		AnimationCurve.EaseInOut(0.2f, 0f, 1f, 1f);

	private ParticleSystem.EmissionModule m_DustEmission;
	private ParticleSystem.VelocityOverLifetimeModule m_DustVelocity;

	private ParticleSystem.EmissionModule m_StreakEmission;
	private ParticleSystem.VelocityOverLifetimeModule m_StreakVelocity;

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
	}

	private void Update()
	{
		float intensity = m_StormController.StormIntensity;

		UpdateDust(intensity);
		UpdateWindStreaks(intensity);
	}

	private void UpdateDust(float intensity)
	{
		float emission =
			m_DustEmissionCurve.Evaluate(intensity)
			* m_MaxDustEmission;

		m_DustEmission.rateOverTime = emission;

		float speed = Mathf.Lerp(
			m_MinDustSpeed,
			m_MaxDustSpeed,
			intensity
		);

		Vector2 wind = m_StormController.WindDirection;

		m_DustVelocity.x = wind.x * speed;
		m_DustVelocity.y = wind.y * speed;
	}

	private void UpdateWindStreaks(float intensity)
	{
		float emission =
			m_StreakEmissionCurve.Evaluate(intensity)
			* m_MaxStreakEmission;

		m_StreakEmission.rateOverTime = emission;

		float speed = Mathf.Lerp(
			m_MinStreakSpeed,
			m_MaxStreakSpeed,
			intensity
		);

		Vector2 wind = m_StormController.WindDirection;

		m_StreakVelocity.x = wind.x * speed;
		m_StreakVelocity.y = wind.y * speed;
	}
}