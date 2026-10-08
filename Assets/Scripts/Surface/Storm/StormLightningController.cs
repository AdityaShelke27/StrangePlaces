using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class StormLightningController : MonoBehaviour
{
	public event Action OnLightningFlash;

	[Header("References")]
	[SerializeField] private StormController m_StormController;
	[SerializeField] private SpriteRenderer m_FlashRenderer;

	[Header("Lightning")]
	[SerializeField, Range(0f, 1f)]
	private float m_MinStormIntensity = 0.7f;

	[SerializeField] private float m_MinInterval = 5f;
	[SerializeField] private float m_MaxInterval = 15f;

	[SerializeField, Range(0f, 1f)]
	private float m_MinFlashAlpha = 0.15f;

	[SerializeField, Range(0f, 1f)]
	private float m_MaxFlashAlpha = 0.35f;

	[SerializeField] private float m_FlashDuration = 0.08f;

	private float m_Timer;
	private float m_NextLightningTime;
	private bool m_IsFlashing;

	private void Start()
	{
		SetFlashAlpha(0f);
		ScheduleNextLightning();
	}

	private void Update()
	{
		if (m_IsFlashing)
			return;

		float intensity =
			m_StormController.StormIntensity;

		if (intensity < m_MinStormIntensity)
		{
			m_Timer = 0f;
			return;
		}

		m_Timer += Time.deltaTime;

		if (m_Timer >= m_NextLightningTime)
		{
			StartCoroutine(
				LightningFlash(intensity)
			);

			ScheduleNextLightning();
		}
	}

	private IEnumerator LightningFlash(float stormIntensity)
	{
		m_IsFlashing = true;

		float normalizedIntensity =
			Mathf.InverseLerp(
				m_MinStormIntensity,
				1f,
				stormIntensity
			);

		float alpha =
			Mathf.Lerp(
				m_MinFlashAlpha,
				m_MaxFlashAlpha,
				normalizedIntensity
			);

		OnLightningFlash?.Invoke();
		// First flash
		SetFlashAlpha(alpha);

		yield return new WaitForSeconds(
			Random.Range(0.04f, 0.09f)
		);

		SetFlashAlpha(0f);

		// Occasionally produce a second flash.
		if (Random.value < 0.35f)
		{
			yield return new WaitForSeconds(
				Random.Range(0.05f, 0.15f)
			);

			SetFlashAlpha(alpha * Random.Range(0.6f, 1f));

			yield return new WaitForSeconds(
				Random.Range(0.04f, 0.08f)
			);

			SetFlashAlpha(0f);
		}

		m_IsFlashing = false;
	}

	private void ScheduleNextLightning()
	{
		m_Timer = 0f;

		float intensity =
			m_StormController.StormIntensity;

		float normalizedIntensity =
			Mathf.InverseLerp(
				m_MinStormIntensity,
				1f,
				intensity
			);

		float minInterval =
			Mathf.Lerp(
				m_MinInterval,
				3f,
				normalizedIntensity
			);

		float maxInterval =
			Mathf.Lerp(
				m_MaxInterval,
				8f,
				normalizedIntensity
			);

		m_NextLightningTime =
			Random.Range(
				minInterval,
				maxInterval
			);
	}

	private void SetFlashAlpha(float alpha)
	{
		Color color =
			m_FlashRenderer.color;

		color.a = alpha;

		m_FlashRenderer.color = color;
	}
}