using UnityEngine;
using NavMeshPlus.Components;
using System;

public class NavMeshManager : MonoBehaviour
{
    public static Action s_BuildNavmesh;
    [SerializeField] private NavMeshSurface m_NavMeshSurface;

    private void OnEnable()
    {
        s_BuildNavmesh += BuildNavmesh;
    }
    private void OnDisable()
    {
        s_BuildNavmesh -= BuildNavmesh;
    }
    void BuildNavmesh()
    {
        m_NavMeshSurface.BuildNavMesh();
    }
}
