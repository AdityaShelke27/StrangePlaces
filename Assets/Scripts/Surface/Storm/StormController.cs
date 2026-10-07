using UnityEngine;

public class StormController : MonoBehaviour
{
	[Header("Storm")]
	[SerializeField] private float m_StormBuildUpDuration = 180f;

	[SerializeField]
	private AnimationCurve m_StormIntensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[Header("Wind")]
	[SerializeField] private Vector2 m_WindDirection = new(1f, -0.25f);
	public Vector2 WindDirection => m_WindDirection.normalized;

	[Header("Debug")]
	[SerializeField] private bool m_DebugMode;

	[Range(0f, 1f)]
	[SerializeField] private float m_DebugStormIntensity;

	private float m_StormTimer;
	private bool m_IsStormApproaching;

	public float StormIntensity { get; private set; }

	private static readonly int StormIntensityID = Shader.PropertyToID("_StormIntensity");

	private static readonly int WindDirectionID = Shader.PropertyToID("_WindDirection");

	private void Start()
	{
		SetStormIntensity(0f);
	}

	private void Update()
	{
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
	}
}