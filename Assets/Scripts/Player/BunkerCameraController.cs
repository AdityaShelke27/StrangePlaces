using UnityEngine;
using UnityEngine.InputSystem;

public class BunkerCameraController : MonoBehaviour
{
	[Header("Zoom")]
	[SerializeField] private float m_MinZoom = 5f;
	[SerializeField] private float m_MaxZoom = 12f;
	[SerializeField] private float m_ZoomSpeed = 1f;

	[Header("Vertical Scroll")]
	[SerializeField] private float m_ScrollSpeed = 0.01f;
	[SerializeField] private float m_MinY = -10f;
	[SerializeField] private float m_MaxY = 10f;

	private Camera m_Camera;

	private Vector2 m_LastPointerPosition;
	private bool m_IsDragging;

	private void Awake()
	{
		m_Camera = GetComponent<Camera>();
	}

	private void Update()
	{
		if (InputManager.GetIsTouchOverGUI()) return;

		HandleZoom();
		HandleVerticalScroll();
	}

	private void HandleZoom()
	{
		// Mouse wheel
		if (Mouse.current != null)
		{
			float scroll = Mouse.current.scroll.ReadValue().y;

			if (Mathf.Abs(scroll) > 0.01f)
			{
				float zoom =
					m_Camera.orthographicSize -
					Mathf.Sign(scroll) * m_ZoomSpeed;

				m_Camera.orthographicSize =
					Mathf.Clamp(zoom, m_MinZoom, m_MaxZoom);
			}
		}

		// Mobile pinch
		if (Touchscreen.current != null)
		{
			var touches = Touchscreen.current.touches;

			if (touches.Count < 2) return;

			var touch0 = touches[0];
			var touch1 = touches[1];

			if (!touch0.press.isPressed || !touch1.press.isPressed) return;

			Vector2 current0 = touch0.position.ReadValue();
			Vector2 current1 = touch1.position.ReadValue();

			Vector2 previous0 = current0 - touch0.delta.ReadValue();
			Vector2 previous1 = current1 - touch1.delta.ReadValue();

			float previousDistance = Vector2.Distance(previous0, previous1);
			float currentDistance = Vector2.Distance(current0, current1);
			float difference = currentDistance - previousDistance;
			float zoom = m_Camera.orthographicSize - difference * m_ZoomSpeed * 0.01f;

			m_Camera.orthographicSize = Mathf.Clamp(zoom, m_MinZoom, m_MaxZoom);
		}
	}

	private void HandleVerticalScroll()
	{
		// Mouse drag
		if (Mouse.current != null)
		{
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				m_LastPointerPosition = Mouse.current.position.ReadValue();
				m_IsDragging = true;
			}

			if (Mouse.current.leftButton.wasReleasedThisFrame)
			{
				m_IsDragging = false;
			}

			if (m_IsDragging)
			{
				Vector2 currentPosition = Mouse.current.position.ReadValue();

				float deltaY = currentPosition.y - m_LastPointerPosition.y;
				MoveVertical(-deltaY);

				m_LastPointerPosition = currentPosition;
			}
		}

		// Single finger drag
		if (Touchscreen.current != null)
		{
			var primaryTouch = Touchscreen.current.primaryTouch;

			if (primaryTouch.press.isPressed)
			{
				// Don't scroll while pinching.
				int activeTouches = 0;

				foreach (var touch in Touchscreen.current.touches)
				{
					if (touch.press.isPressed)
						activeTouches++;
				}

				if (activeTouches == 1)
				{
					float deltaY =
						primaryTouch.delta.ReadValue().y;

					MoveVertical(-deltaY);
				}
			}
		}
	}

	private void MoveVertical(float deltaY)
	{
		Vector3 position = transform.position;

		position.y += deltaY * m_ScrollSpeed;

		position.y = Mathf.Clamp(
			position.y,
			m_MinY,
			m_MaxY
		);

		// X is never modified.
		transform.position = position;
	}
}