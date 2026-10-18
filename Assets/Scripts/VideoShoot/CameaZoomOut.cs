using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameaZoomOut : MonoBehaviour
{
	public static Action m_StartGeneration;

	[SerializeField] float m_TotalTime;
	[SerializeField] float m_TargetFov;
	Camera m_Camera;

	private void OnEnable()
	{
		m_StartGeneration += VideoShoot;
	}
	private void OnDisable()
	{
		m_StartGeneration -= VideoShoot;
	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        m_Camera = GetComponent<Camera>();
		
	}

    // Update is called once per frame
    void Update()
    {
		//if (Keyboard.current.wKey.isPressed)
		//	m_StartGeneration?.Invoke();
	}

	void VideoShoot()
	{
		StartCoroutine(LerpCamera());
	}
	IEnumerator LerpCamera()
	{
		float _initialFOV = m_Camera.orthographicSize;
		float _counter = 0;
		float _diff = m_TargetFov - _initialFOV;

		//yield return new WaitForSeconds(2);

		while(_counter < m_TotalTime)
		{
			m_Camera.orthographicSize = _initialFOV + _diff * (_counter / m_TotalTime);
			_counter += Time.deltaTime;

			yield return null;
		}
	}
}
