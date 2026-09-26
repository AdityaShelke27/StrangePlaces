using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
	public static ConveyorManager Instance;
	public static Action s_StartConveyorMode;
	public static Action s_EndConveyorMode;
	[SerializeField] Camera m_Camera;
	bool m_IsInConveyorMode = false;

	Vector3 m_StartPos;
	Vector3 m_EndPos;

	[SerializeField] List<Vector2> m_ConveyorPath;

	private void Awake()
	{
		if(Instance == null) Instance = this;
	}
	private void OnEnable()
	{
		s_StartConveyorMode += StartConveyorMode;
	}
	private void OnDisable()
	{
		s_StartConveyorMode -= StartConveyorMode;
	}
	void StartConveyorMode()
	{
		m_IsInConveyorMode = true;

		Debug.Log("Conveyor Active");
		StartCoroutine(UpdateConveyorPath());
	}

	IEnumerator UpdateConveyorPath()
	{
		while (m_IsInConveyorMode)
		{
			m_EndPos = m_Camera.ScreenToWorldPoint(InputManager.GetTouchPosition());
			if (m_StartPos == null || m_EndPos == null) continue;

			m_ConveyorPath = GetConveyorPath(m_StartPos, m_EndPos);
			yield return null;
		}
	}
	public List<Vector2> GetConveyorPath(Vector2 start, Vector2 end)
	{
		List<Vector2> points = new()
		{
			start
		};

		// Already horizontally or vertically aligned
		if (Mathf.Approximately(start.y, end.y) || Mathf.Approximately(start.x, end.x))
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
	public void SetStartPos(Vector3 _pos) => m_StartPos = _pos;
	public void SetEndPos(Vector3 _pos) => m_EndPos = _pos;
}
