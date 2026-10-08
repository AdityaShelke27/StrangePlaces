using UnityEngine;

public class StormAtmosphereController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private StormController m_StormController;
	[SerializeField] private Camera m_Camera;
	[SerializeField] private SpriteRenderer m_DarkeningRenderer;

	[Header("Darkening")]
	[SerializeField]
	private Color m_StormColor =
		new Color(0.08f, 0.10f, 0.13f, 1f);

	[Range(0f, 1f)]
	[SerializeField] private float m_MaxDarkness = 0.35f;

	[SerializeField]
	private AnimationCurve m_DarknessCurve =
		AnimationCurve.EaseInOut(
			0f, 0f,
			1f, 1f
		);

	[Header("Coverage")]
	[SerializeField] private float m_CoverageMargin = 2f;

	private void Awake()
	{
		UpdateSize();
	}

	private void Update()
	{
		UpdateDarkness();
	}

	private void LateUpdate()
	{
		FollowCamera();
	}

	private void UpdateDarkness()
	{
		float intensity =
			m_StormController.StormIntensity;

		float darkness =
			m_DarknessCurve.Evaluate(intensity);

		Color color = m_StormColor;

		color.a =
			darkness *
			m_MaxDarkness;

		m_DarkeningRenderer.color = color;
	}

	private void FollowCamera()
	{
		Vector3 position = m_Camera.transform.position;

		position.z = transform.position.z;

		transform.position = position;

		UpdateSize();
	}

	private void UpdateSize()
	{
		if (!m_Camera.orthographic)
			return;

		Sprite sprite =
			m_DarkeningRenderer.sprite;

		if (sprite == null)
			return;

		float cameraHeight =
			m_Camera.orthographicSize * 2f;

		float cameraWidth =
			cameraHeight *
			m_Camera.aspect;

		cameraWidth += m_CoverageMargin;
		cameraHeight += m_CoverageMargin;

		Vector2 spriteSize =
			sprite.bounds.size;

		transform.localScale =
			new Vector3(
				cameraWidth / spriteSize.x,
				cameraHeight / spriteSize.y,
				1f
			);
	}
}