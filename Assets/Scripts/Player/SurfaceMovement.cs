using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SurfaceMovement : MonoBehaviour
{
	public static Action<GameObject> s_Selected;
	public static SurfaceMovement Instance;

	[SerializeField] SpriteRenderer m_PlayerRenderer;
	[SerializeField] Animator m_Animator;
	[SerializeField] Transform m_MainCam;
	[SerializeField] Transform m_Bunker;
	[SerializeField] RectTransform m_BunkerPointer;
	[SerializeField] float m_CamMoveSpeed;
	Camera m_Camera;
	private NavMeshAgent agent;
	bool m_IsMoving = false;
	bool m_IsHasWork = false;

	Vector2 SCREEN_SIZE = new(Screen.width, Screen.height);
	Vector2 SCREEN_CENTER = new(Screen.width / 2, Screen.height / 2);
	float m_EdgePadding = 60f;

	IActivate m_SelectedObject;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}
	void Start()
	{
		//PlayerData.LoadData();
		//PlayerStatsManager.Instance.SetElectricity(PlayerData.electricity);
		//PlayerStatsManager.Instance.SetHunger(PlayerData.hunger);
		//PlayerStatsManager.Instance.SetResearchPoints(PlayerData.researchPoints);

		m_Camera = m_MainCam.GetComponent<Camera>();
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
			m_PlayerRenderer.flipX = agent.velocity.x < 0;
			if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
			{
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
				{
					if (m_SelectedObject != null)
					{
						m_SelectedObject.Activate();
						m_SelectedObject = null;
						m_IsHasWork = false;
					}
					m_IsMoving = false;
				}
			}
		}
		m_Animator.SetBool(Constant.PLAYER_RUN, m_IsMoving);

		PointToBunker();
	}
	void UpdatePosition(InputAction.CallbackContext ctx)
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (ConveyorManager.Instance.GetIsInConveyorMode()) return;

		if(m_IsHasWork)
		{
			m_SelectedObject = null;
			m_IsHasWork = false;
		}
		agent.SetDestination(Camera.main.ScreenToWorldPoint(InputManager.GetTouchPosition()));
		m_IsMoving = true;
	}
	public void MoveTo(Vector3 pos)
	{
		agent.SetDestination(pos);
		m_IsMoving = true;
	}
	public void MoveToKeepDistance(Vector3 pos, float distance)
	{
		Vector3 _dir = (pos - agent.transform.position).normalized;
		float _dist = (pos - agent.transform.position).magnitude;

		agent.SetDestination(agent.transform.position + (_dir * (_dist - distance)));
		m_IsMoving = true;
		m_IsHasWork = true;
	}
	void PointToBunker()
	{
		Vector3 screenPos = m_Camera.WorldToScreenPoint(m_Bunker.position);

		bool isBehindCamera = screenPos.z < 0;

		bool isVisible =
			screenPos.x >= 0 &&
			screenPos.x <= SCREEN_SIZE[0] &&
			screenPos.y >= 0 &&
			screenPos.y <= SCREEN_SIZE[1] &&
			!isBehindCamera;

		m_BunkerPointer.gameObject.SetActive(!isVisible);

		if (!isVisible)
		{
			UpdateArrow(screenPos);
		}
	}
	private void UpdateArrow(Vector3 bunkerScreenPos)
	{
		Vector2 direction = (Vector2)bunkerScreenPos - SCREEN_CENTER;

		// If bunker is behind the camera, reverse the direction.
		if (bunkerScreenPos.z < 0)
			direction = -direction;

		direction.Normalize();

		// Distance from screen center to the edge.
		float halfWidth = Screen.width * 0.5f - m_EdgePadding;
		float halfHeight = Screen.height * 0.5f - m_EdgePadding;

		float scaleX = halfWidth / Mathf.Abs(direction.x);
		float scaleY = halfHeight / Mathf.Abs(direction.y);

		float distance = Mathf.Min(scaleX, scaleY);

		Vector2 arrowScreenPos = SCREEN_CENTER + direction * distance;

		m_BunkerPointer.position = arrowScreenPos;
		//m_BunkerPointer.position = Vector3.Lerp(
		//	m_BunkerPointer.position,
		//	arrowScreenPos,
		//	Time.deltaTime * 10f
		//);

		// Rotate arrow so its UP direction points toward bunker.
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		m_BunkerPointer.rotation = Quaternion.Euler(
			0f,
			0f,
			angle - 90f
		);
	}
	public Animator GetAnimator() => m_Animator;
	void SelectObject(GameObject _Obj) => m_SelectedObject = _Obj.GetComponent<IActivate>();
}
