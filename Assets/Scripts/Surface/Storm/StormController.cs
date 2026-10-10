using System.Collections;
using UnityEngine;

public class StormController : MonoBehaviour
{
	[Header("Storm")]
	[SerializeField] private float m_StormBuildUpMin = 240f;
	[SerializeField] private float m_StormBuildUpMax = 360f;
	private float m_StormBuildUpDuration = 180f;

	[SerializeField]
	private AnimationCurve m_StormIntensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[Header("Wind")]
	[SerializeField] private Vector2 m_WindDirection = new(1f, -0.25f);
	public Vector2 WindDirection => m_WindDirection.normalized;

	[Header("Debug")]
	[SerializeField] private bool m_DebugMode;

	[Range(0f, 1f)]
	[SerializeField] private float m_DebugStormIntensity;

	[Header("Gusts")]
	[SerializeField] private float m_MinGustInterval = 4f;
	[SerializeField] private float m_MaxGustInterval = 10f;

	[SerializeField] private float m_MinGustDuration = 0.5f;
	[SerializeField] private float m_MaxGustDuration = 1.5f;

	[SerializeField] private AnimationCurve m_GustCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[Header("Player Alive Time")]
	[SerializeField] private float m_MinPlayerAdditionalAliveTime = 5;
	[SerializeField] private float m_MaxPlayerAdditionalAliveTime = 15;

	private float m_GustTimer;
	private float m_NextGustTime;

	private float m_CurrentGustDuration;
	private bool m_IsGusting;

	private float m_StormTimer;
	private bool m_IsStormApproaching;

	public float StormIntensity { get; private set; }
	public float GustIntensity { get; private set; }

	private static readonly int StormIntensityID = Shader.PropertyToID("_StormIntensity");
	private static readonly int WindDirectionID = Shader.PropertyToID("_WindDirection");
	private static readonly int GustIntensityID = Shader.PropertyToID("_GustIntensity");

	[SerializeField] float m_PlayerAliveTimer; 

	private void Start()
	{
		m_StormBuildUpDuration = Random.Range(m_StormBuildUpMin, m_StormBuildUpMax);
		m_PlayerAliveTimer = Random.Range(m_MinPlayerAdditionalAliveTime, m_MaxPlayerAdditionalAliveTime);

		StartStorm();
		SetStormIntensity(0f);
		ScheduleNextGust();
	}

	private void Update()
	{
		UpdateGusts();
		if (m_DebugMode)
		{
			SetStormIntensity(m_DebugStormIntensity);
			return;
		}

		if (!m_IsStormApproaching) return;

		m_StormTimer += Time.deltaTime;

		float progress = Mathf.Clamp01(m_StormTimer / m_StormBuildUpDuration);
		float intensity = m_StormIntensityCurve.Evaluate(progress);

		SetStormIntensity(intensity);

		if (progress >= 1f)
		{
			m_IsStormApproaching = false;
			SetStormIntensity(1f);

			OnFullStormReached();
		}
		
	}

	public void StartStorm()
	{
		m_StormTimer = 0f;
		m_IsStormApproaching = true;

		SetStormIntensity(0f);
	}

	public void SetStormProgress(float progress)
	{
		progress = Mathf.Clamp01(progress);

		float intensity =
			m_StormIntensityCurve.Evaluate(progress);

		SetStormIntensity(intensity);
	}

	private void SetStormIntensity(float intensity)
	{
		StormIntensity = Mathf.Clamp01(intensity);

		Shader.SetGlobalFloat(
			StormIntensityID,
			StormIntensity
		);

		Vector2 direction = m_WindDirection.normalized;

		Shader.SetGlobalVector(
			WindDirectionID,
			new Vector4(
				direction.x,
				direction.y,
				0f,
				0f
			)
		);
	}

	private void OnFullStormReached()
	{
		Debug.Log("Full storm reached.");
		StartCoroutine(PlayerAdditionalAliveTime());
	}
	IEnumerator PlayerAdditionalAliveTime()
	{
		while(m_PlayerAliveTimer > 0)
		{
			yield return null;
			m_PlayerAliveTimer -= Time.deltaTime;
		}

		Bunker.s_PlayerFailed?.Invoke();
	}
	private void UpdateGusts()
	{
		if (StormIntensity < 0.25f)
		{
			GustIntensity = 0f;
			return;
		}

		if (!m_IsGusting)
		{
			m_GustTimer += Time.deltaTime;

			if (m_GustTimer >= m_NextGustTime)
			{
				StartGust();
			}

			return;
		}

		m_GustTimer += Time.deltaTime;
		float progress = Mathf.Clamp01(m_GustTimer / m_CurrentGustDuration);

		// Rise and fall.
		float wave = Mathf.Sin(progress * Mathf.PI);
		GustIntensity = wave * StormIntensity;

		Shader.SetGlobalFloat(GustIntensityID, GustIntensity);

		if (progress >= 1f)
		{
			m_IsGusting = false;
			GustIntensity = 0f;

			ScheduleNextGust();
		}
	}

	private void StartGust()
	{
		m_IsGusting = true;
		m_GustTimer = 0f;

		m_CurrentGustDuration =
			Random.Range(
				m_MinGustDuration,
				m_MaxGustDuration
			);
	}

	private void ScheduleNextGust()
	{
		m_IsGusting = false;
		m_GustTimer = 0f;

		m_NextGustTime =
			Random.Range(
				m_MinGustInterval,
				m_MaxGustInterval
			);
	}
}