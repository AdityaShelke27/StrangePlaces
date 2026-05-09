using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform m_MainCam;
    [SerializeField] float m_CamMoveSpeed;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void OnEnable()
    {
        InputManager.OnTap += UpdatePosition;
    }
    private void OnDisable()
    {
        InputManager.OnTap -= UpdatePosition;
    }

    void Update()
    {
	    Vector3 pos = Vector3.Lerp(m_MainCam.transform.position, transform.position, Time.deltaTime * m_CamMoveSpeed);
        pos.z = -10;
        m_MainCam.transform.position = pos;
    }
    void UpdatePosition(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        agent.SetDestination(Camera.main.ScreenToWorldPoint(InputManager.GetTouchPosition()));
    }
}
