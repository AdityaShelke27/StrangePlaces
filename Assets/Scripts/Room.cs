using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] int m_GroundLevel;
    private void OnMouseDown()
    {
        Debug.Log("Tapped");
        //BunkerMovement.s_MoveHere?.Invoke(m_GroundLevel, );
    }
}
