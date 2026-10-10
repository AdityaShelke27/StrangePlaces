using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static Action<InputAction.CallbackContext> OnTap;
	public static Action<InputAction.CallbackContext> OnPressStart;
	public static Action<InputAction.CallbackContext> OnPressEnd;

	static PlayerInputActions input;

	static bool IsTouchPressed = false;
	static bool IsTouchOverGUI = false;
	static float DragThreshold = 20;

	static Vector2 TouchStartPos;
	static Vector2 TouchEndPos;
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
	void HandleTap(InputAction.CallbackContext ctx) 
	{
		if (IsPointerOverUI()) return;

		OnTap?.Invoke(ctx); 
	}
	void HandlePressStart(InputAction.CallbackContext ctx) 
	{
		if(IsPointerOverUI())
		{
			IsTouchOverGUI = true;
			return;
		}
		TouchStartPos = GetTouchPosition();
		IsTouchPressed = true;
		OnPressStart?.Invoke(ctx); 
	}
	void HandlePressEnd(InputAction.CallbackContext ctx) 
	{
		TouchEndPos = GetTouchPosition();
		IsTouchPressed = false;
		IsTouchOverGUI = false;
		OnPressEnd?.Invoke(ctx); 
	}
	public static Vector2 GetTouchPosition() => input.Touch.Position.ReadValue<Vector2>();
	public static bool GetIsTouchPressed() => IsTouchPressed;

	public static bool IsPointerOverUI()
	{
		if (EventSystem.current == null)
			return false;

		PointerEventData pointerData =
			new PointerEventData(EventSystem.current)
			{
				position = GetTouchPosition()
			};

		List<RaycastResult> results = new();

		EventSystem.current.RaycastAll(pointerData, results);

		return results.Count > 0;
	}
	public static bool GetIsTouchOverGUI() => IsTouchOverGUI;
	public static bool IsADrag() => Vector2.Distance(TouchStartPos, TouchEndPos) > DragThreshold;
}
