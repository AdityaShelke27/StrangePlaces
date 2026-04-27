using System.Collections;
using UnityEngine;

public class ResourceNodeInstance : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_SpriteRenderer;
    [SerializeField] ResourceNode m_ResourceNodeData;
    [SerializeField] SurfaceNodeAmount m_NodeAmount;
    int m_MaxAmount;
    [SerializeField] int m_AmountAvailable;
    bool m_AllResourcesDepleted = false;

    void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        m_SpriteRenderer.sprite = m_ResourceNodeData.itemImage;
        m_MaxAmount = m_ResourceNodeData.MaxAmount;
        m_AmountAvailable = m_MaxAmount;
    }
    public int FetchResource(int amount)
    {
        if (m_AllResourcesDepleted) return 0;

        if(amount < m_AmountAvailable)
        {
            m_AmountAvailable -= amount;
        }
        else
        {
            amount = m_AmountAvailable;
            m_AmountAvailable = 0;
            m_AllResourcesDepleted = true;
            StartCoroutine(DestroyNodeNextFrame());
        }

        return amount;
    }
    IEnumerator DestroyNodeNextFrame()
    {
        yield return null;

        Destroy(gameObject);
    }
    public ResourceNode GetResourceNodeData() => m_ResourceNodeData;
}
