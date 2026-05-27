using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static Action<InputAction.CallbackContext> OnTap;
	public static Action<InputAction.CallbackContext> OnPressStart;
	public static Action<InputAction.CallbackContext> OnPressEnd;

	static PlayerInputActions input;

	static bool IsTouchPressed = false;
	private void Awake()
	{
		input = new();
	}

	private void OnEnable()
	{
		input.Enable();
		input.Touch.Tap.performed += HandleTap;
		input.Touch.Press.started += HandlePressStart;
		input.Touch.Press.canceled += HandlePressEnd;
	}
	private void OnDisable()
	{
		input.Disable();
		input.Touch.Tap.performed -= HandleTap;
		input.Touch.Press.started -= HandlePressStart;
		input.Touch.Press.canceled -= HandlePressEnd;
	}
	void HandleTap(InputAction.CallbackContext ctx) => OnTap?.Invoke(ctx);
	void HandlePressStart(InputAction.CallbackContext ctx) 
	{
		IsTouchPressed = true;
		OnPressStart?.Invoke(ctx); 
	}
	void HandlePressEnd(InputAction.CallbackContext ctx) 
	{
		IsTouchPressed = false;
		OnPressEnd?.Invoke(ctx); 
	}
	public static Vector2 GetTouchPosition() => input.Touch.Position.ReadValue<Vector2>();
	public static bool GetIsTouchPressed() => IsTouchPressed;
}
