using System;
using System.Collections;
using UnityEngine;

public class BunkerMovement : MonoBehaviour
{
    public static Action<int, float> s_MoveHere;

    [SerializeField] Transform[] m_Points;
    [SerializeField] int m_CurrentGroundLevel = 0;
    [SerializeField] float m_Speed = 10;

    private void OnEnable()
    {
        s_MoveHere += MoveToPoint;
    }
    private void OnDisable()
    {
        s_MoveHere -= MoveToPoint;
    }
    void MoveToPoint(int _groundLevel, float _pointX)
    {
        StartCoroutine(Cor_MoveToPoint(_groundLevel, _pointX));
    }
    IEnumerator Cor_MoveToPoint(int _groundLevel, float _pointX)
    {
        Vector2 _movePos = m_Points[m_CurrentGroundLevel].position;
        Vector2 _dir = (_movePos - (Vector2)transform.position).normalized;
        while (Vector2.Distance(transform.position, _movePos) > 0.01f)
        {
            transform.Translate(m_Speed * Time.deltaTime * _dir);
            yield return null;
        }

        _movePos = m_Points[_groundLevel].position;
        _dir = (_movePos - (Vector2)transform.position).normalized;
        while (Vector2.Distance(transform.position, _movePos) > 0.01f)
        {
            transform.Translate(m_Speed * Time.deltaTime * _dir);
            yield return null;
        }

        _movePos = new Vector2(_pointX, m_Points[_groundLevel].position.y);
        _dir = (_movePos - (Vector2)transform.position).normalized;
        while (Vector2.Distance(transform.position, _movePos) > 0.01f)
        {
            transform.Translate(m_Speed * Time.deltaTime * _dir);
            yield return null;
        }
    }
}
