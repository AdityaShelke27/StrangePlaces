using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConveyorManager : MonoBehaviour
{
	public static ConveyorManager Instance;

	public static Action s_StartConveyorMode;
	public static Action s_EndConveyorMode;

	[SerializeField] Image m_ConveyorButtonImage;
	[SerializeField] GameObject m_ConveyorConnectionPrefab;
	[SerializeField] Camera m_Camera;
	[SerializeField] LayerMask m_SocketLayer;

	bool m_IsInConveyorMode = false;

	ConveyorSocket m_StartSocket;
	ConveyorSocket m_HoveredSocket;

	ConveyorConnection m_CurrentConnection;

	[SerializeField] List<Vector2> m_ConveyorPath;

	private void Awake()
	{
		if(Instance == null) Instance = this;
	}
	private void OnEnable()
	{
		s_StartConveyorMode += StartConveyorMode;
		s_EndConveyorMode += EndConveyorMode;
	}
	private void OnDisable()
	{
		s_StartConveyorMode -= StartConveyorMode;
		s_EndConveyorMode += EndConveyorMode;
	}
	public void ToggleConvetModeButton()
	{
		if(m_IsInConveyorMode) s_EndConveyorMode?.Invoke();
		else s_StartConveyorMode?.Invoke();
	}
	void StartConveyorMode()
	{
		m_IsInConveyorMode = true;
		m_ConveyorButtonImage.color = Color.green;

		Debug.Log("Conveyor Active");
	}
	void EndConveyorMode()
	{
		m_IsInConveyorMode = false;
		m_ConveyorButtonImage.color = Color.white;

		Debug.Log("Conveyor Inactive");
	}
	public void BeginConnection(ConveyorSocket _socket)
	{
		if (!GetIsInConveyorMode()) return;
		if (_socket.IsInputSocket) return;
		if (_socket.GetSocketConnected()) return;

		m_StartSocket = _socket;

		GameObject obj = Instantiate(m_ConveyorConnectionPrefab, Vector3.zero, Quaternion.identity);
		m_CurrentConnection = obj.GetComponent<ConveyorConnection>();

		StartCoroutine(UpdateConveyorPath());
	}
	IEnumerator UpdateConveyorPath()
	{
		while (m_StartSocket != null)
		{
			Vector2 _pointerPos = m_Camera.ScreenToWorldPoint(InputManager.GetTouchPosition());

			m_HoveredSocket = GetSocketAtPosition(_pointerPos);
			Vector2 _endPos = _pointerPos;

			// Snap preview to valid input socket
			if (m_HoveredSocket != null && m_HoveredSocket.IsInputSocket && CheckConnectionCompatibility() && !m_HoveredSocket.GetSocketConnected())
			{
				_endPos = m_HoveredSocket.transform.position;
			}

			Vector2 _startPos = m_StartSocket.transform.position;
			m_ConveyorPath = GetConveyorPath(_startPos, _endPos);
			m_CurrentConnection.SetPreview(m_ConveyorPath);

			// Finger/mouse released
			if (!InputManager.GetIsTouchPressed())
			{
				FinishConnection();
				yield break;
			}

			yield return null;
		}
	}
	ConveyorSocket GetSocketAtPosition(Vector2 _pos)
	{
		Collider2D _hit = Physics2D.OverlapPoint(_pos, m_SocketLayer);

		if (_hit == null) return null;

		return _hit.GetComponent<ConveyorSocket>();
	}
	void FinishConnection()
	{
		if(m_HoveredSocket == null || !m_HoveredSocket.IsInputSocket || !CheckConnectionCompatibility() || m_HoveredSocket.GetSocketConnected())
		{
			Destroy(m_CurrentConnection.gameObject);
		}
		else
		{
			// Valid connection
			Debug.Log($"Connected {m_StartSocket.name} -> {m_HoveredSocket.name}");

			m_CurrentConnection.Initialize(m_StartSocket, m_HoveredSocket);
		}

		m_StartSocket = null;
		m_HoveredSocket = null;
		m_CurrentConnection = null;
	}
	public List<Vector2> GetConveyorPath(Vector2 start, Vector2 end)
	{
		List<Vector2> points = new()
		{
			start
		};

		// Already horizontally or vertically aligned
		if (Mathf.Abs(start.y - end.y) < Constant.CONVEYOR_ALIGNMENT_THRESHOLD || Mathf.Abs(start.x - end.x) < Constant.CONVEYOR_ALIGNMENT_THRESHOLD)
		{
			points.Add(end);
			return points;
		}

		float midX = (start.x + end.x) * 0.5f;

		points.Add(new Vector2(midX, start.y));
		points.Add(new Vector2(midX, end.y));
		points.Add(end);

		return points;
	}
	bool CheckConnectionCompatibility()
	{
		if (m_HoveredSocket.IsBunkerSocket) return true;
		if (m_StartSocket.GetMachineScript().gameObject == m_HoveredSocket.GetMachineScript().gameObject) return false;
		return m_HoveredSocket.GetMachineScript().IsItemAddable(m_StartSocket.GetMachineScript().GetCurrentResourceOutput());
	}
	private void OnDrawGizmos()
	{
		if(m_ConveyorPath == null) return;

		foreach(Vector3 _pos in m_ConveyorPath)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(_pos, 0.1f);
		}
	}
	public bool GetIsInConveyorMode() => m_IsInConveyorMode;
}
