using UnityEngine;

public class ResourceNodeTemp : MonoBehaviour
{
    [SerializeField] SurfaceNode m_NodeType;
    [SerializeField] SurfaceNodeAmount m_NodeAmount;
    int m_MaxAmount;
    int m_AmountAvailable;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        
    }
    void Initialize()
    {
        m_MaxAmount = ResourceData.ResourceAmount[m_NodeAmount];
        m_AmountAvailable = m_MaxAmount;
    }
    public int FetchResource(int amount)
    {
        if(amount >= m_AmountAvailable)
        {
            m_AmountAvailable -= amount;
        }
        else
        {
            amount = m_AmountAvailable;
            m_AmountAvailable = 0;
        }

        return amount;
    }

    private void OnMouseDown()
    {
        ResourceHandler.Instance.NodeSelected(this);
    }
    public SurfaceNode GetNodeType()
    {
        return m_NodeType;
    }
}
