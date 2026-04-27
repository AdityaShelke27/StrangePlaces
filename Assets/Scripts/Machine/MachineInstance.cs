using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MachineInstance : WorldInstance
{
    [SerializeField] protected SpriteRenderer m_SpriteRenderer;
    [SerializeField] protected GameObject m_InventoryPanel;
    [SerializeField] protected Transform m_OutputSlotListParent;
    [SerializeField] protected GameObject m_InventorySlotPrefab;
    protected bool IsWorking;
    protected MachineState State = MachineState.Inactive;
    public abstract void StartMachine();

    public MachineState GetMachineState() => State;
    public abstract void SetMachineState(MachineState _state);

    public void CloseButton()
    {
        m_InventoryPanel.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        m_InventoryPanel.SetActive(true);
    }
}
