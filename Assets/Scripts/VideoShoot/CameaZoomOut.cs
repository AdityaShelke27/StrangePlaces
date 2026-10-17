using System.Collections;
using UnityEngine;

public class CameaZoomOut : MonoBehaviour
{
	[SerializeField] float m_TotalTime;
	[SerializeField] float m_TargetFov;
	Camera m_Camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Camera = GetComponent<Camera>();
		StartCoroutine(LerpCamera());
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	IEnumerator LerpCamera()
	{
		float _initialFOV = m_Camera.orthographicSize;
		float _counter = 0;
		float _diff = m_TargetFov - _initialFOV;

		yield return new WaitForSeconds(2);

		while(_counter < m_TotalTime)
		{
			m_Camera.orthographicSize = _initialFOV + _diff * (_counter / m_TotalTime);
			_counter += Time.deltaTime;

			yield return null;
		}
	}
}
