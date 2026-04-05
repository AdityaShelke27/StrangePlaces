using System;
using System.Collections;
using UnityEngine;

public class NodeMachineInstance : MachineInstance
{
    [SerializeField] private NodeMachine m_MachineData;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    private ResourceNodeInstance m_Input;
    private ItemSlot m_Output;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Initialize(StorableItem data)
    {
        m_MachineData = data as NodeMachine;
        m_SpriteRenderer.sprite = m_MachineData.itemImage;
    }
    public void SetInput(ResourceNodeInstance _input)
    {
        m_Input = _input;
    }

    public override void StartMachine()
    {
        if(!m_Input)
        {
            Debug.LogWarning("Input Empty");
            return;
        }

        StartCoroutine(MachineWork());
    }

    IEnumerator MachineWork()
    {
        State = MachineState.Working;

        while(m_Input)
        {
            yield return new WaitForSeconds(TimeToProduce);
            int amount = m_Input.FetchResource(1);
        }
    }

}
