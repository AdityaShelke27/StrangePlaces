using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform m_MainCam;
    [SerializeField] float m_CamMoveSpeed;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            UpdatePosition();
        }

        Vector3 pos = Vector3.Lerp(m_MainCam.transform.position, transform.position, Time.deltaTime * m_CamMoveSpeed);
        pos.z = -10;
        m_MainCam.transform.position = pos;
    }
    void UpdatePosition()
    {
        agent.SetDestination(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
