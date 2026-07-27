using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SurfaceMovement : MonoBehaviour
{
	public static Action<GameObject> s_Selected;

	[SerializeField] Transform m_MainCam;
	[SerializeField] float m_CamMoveSpeed;
	private NavMeshAgent agent;
	bool m_IsMoving = false;

	IActivate m_SelectedObject;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
	}
	private void OnEnable()
	{
		InputManager.OnTap += UpdatePosition;
		s_Selected += SelectObject;
	}
	private void OnDisable()
	{
		InputManager.OnTap -= UpdatePosition;
		s_Selected -= SelectObject;
	}

	void Update()
	{
		Vector3 pos = Vector3.Lerp(m_MainCam.position, transform.position, Time.deltaTime * m_CamMoveSpeed);
		pos.z = -10;
		m_MainCam.position = pos;


		if(m_IsMoving)
		{
			if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
			{
				if (m_SelectedObject != null)
				{
					m_SelectedObject.Activate();
					m_SelectedObject = null;
				}
				m_IsMoving = false;
			}
		}
	}
	void UpdatePosition(InputAction.CallbackContext ctx)
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;

		agent.SetDestination(Camera.main.ScreenToWorldPoint(InputManager.GetTouchPosition()));
		m_IsMoving = true;
	}
	void SelectObject(GameObject _Obj) => m_SelectedObject = _Obj.GetComponent<IActivate>();
}
