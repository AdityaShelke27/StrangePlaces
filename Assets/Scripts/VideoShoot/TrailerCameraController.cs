using UnityEngine;
using UnityEngine.InputSystem;

public class TrailerCameraController : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float m_MoveSpeed = 6f;
	[SerializeField] private float m_SmoothTime = 0.2f;

	[Header("Optional Boost")]
	[SerializeField] private float m_BoostMultiplier = 2f;

	private Vector3 m_CurrentVelocity;

	private void Update()
	{
		Vector2 input = GetMovementInput();

		float speed = m_MoveSpeed;

		if (Keyboard.current.leftShiftKey.isPressed)
			speed *= m_BoostMultiplier;

		Vector3 targetVelocity = new Vector3(
			input.x,
			input.y,
			0f
		) * speed;

		m_CurrentVelocity = Vector3.Lerp(
			m_CurrentVelocity,
			targetVelocity,
			1f - Mathf.Exp(-Time.deltaTime / m_SmoothTime)
		);

		transform.position += m_CurrentVelocity * Time.deltaTime;
	}

	private Vector2 GetMovementInput()
	{
		if (Keyboard.current == null)
			return Vector2.zero;

		Vector2 input = Vector2.zero;

		if (Keyboard.current.wKey.isPressed)
			input.y += 1f;

		if (Keyboard.current.sKey.isPressed)
			input.y -= 1f;

		if (Keyboard.current.aKey.isPressed)
			input.x -= 1f;

		if (Keyboard.current.dKey.isPressed)
			input.x += 1f;

		return input.normalized;
	}
}