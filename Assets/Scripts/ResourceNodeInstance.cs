using UnityEngine;

public class ResourceNodeInstance : MonoBehaviour
{
    [SerializeField] ResourceNode m_ResourceNodeData;
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
        m_MaxAmount = m_ResourceNodeData.MaxAmount;
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

    public ResourceNode GetResourceNodeData() => m_ResourceNodeData;
}
