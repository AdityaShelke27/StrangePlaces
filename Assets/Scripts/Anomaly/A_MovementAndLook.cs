using UnityEngine;

public class A_MovementAndLook : MonoBehaviour
{
    [SerializeField] float m_Speed;
    [SerializeField] float m_Sensitivity;
    [SerializeField] Transform m_Camera;
    float yRot = 0;

    void Update()
    {
        Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized);
        Look(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).normalized);
    }

    void Move(Vector2 move)
    {
        transform.position += m_Speed * Time.deltaTime * new Vector3(move.x, 0, move.y);
    }
    void Look(Vector2 look)
    {
        float xRot = transform.eulerAngles.y;
        look *= Time.deltaTime * m_Sensitivity;
        xRot += look.x;

        transform.rotation = Quaternion.Euler(0, xRot, 0);

        yRot -= look.y;
        yRot = Mathf.Clamp(yRot, -89, 89);

        m_Camera.localRotation = Quaternion.Euler(yRot, 0, 0);
    }
}
