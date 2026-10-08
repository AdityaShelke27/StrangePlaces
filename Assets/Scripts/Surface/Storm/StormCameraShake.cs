using UnityEngine;

public class StormCameraShake : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private StormController m_StormController;

	[Header("Gust Shake")]
	[SerializeField] private float m_MinStormIntensity = 0.65f;

	[SerializeField] private float m_MaxPositionStrength = 0.06f;

	[SerializeField] private float m_ShakeSpeed = 18f;

	[Header("Smoothing")]
	[SerializeField] private float m_ReturnSpeed = 10f;

	[SerializeField]
	private StormLightningController m_LightningController;

	[SerializeField]
	private float m_LightningKickStrength = 0.08f;

	private float m_LightningKick;

	private Vector3 m_CurrentOffset;

	private void OnEnable()
	{
		if (m_LightningController != null)
			m_LightningController.OnLightningFlash += OnLightningFlash;
	}

	private void OnDisable()
	{
		if (m_LightningController != null)
			m_LightningController.OnLightningFlash -= OnLightningFlash;
	}

	private void OnLightningFlash()
	{
		m_LightningKick =
			m_LightningKickStrength;
	}

	private void LateUpdate()
	{
		float stormIntensity =
			m_StormController.StormIntensity;

		float gustIntensity =
			m_StormController.GustIntensity;

		float stormFactor =
			Mathf.InverseLerp(
				m_MinStormIntensity,
				1f,
				stormIntensity
			);

		float shakeStrength = 0f;

		if (stormIntensity >= m_MinStormIntensity)
		{
			shakeStrength =
				gustIntensity *
				m_MaxPositionStrength;
		}

		shakeStrength += m_LightningKick;

		Vector3 offset = Vector3.zero;

		if (shakeStrength > 0.001f)
		{
			float time =
				Time.time * m_ShakeSpeed;

			float x =
				(Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f;

			float y =
				(Mathf.PerlinNoise(0f, time) - 0.5f) * 2f;

			offset =
				new Vector3(
					x,
					y,
					0f
				) * shakeStrength;
		}

		transform.localPosition = offset;

		m_LightningKick =
			Mathf.MoveTowards(
				m_LightningKick,
				0f,
				Time.deltaTime * 0.8f
			);
	}
}