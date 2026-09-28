using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorConnection : MonoBehaviour
{
	[SerializeField] SpriteRenderer m_StartStraightConveyor;
	[SerializeField] SpriteRenderer m_UpperCornerConveyor;
	[SerializeField] SpriteRenderer m_MidStraightConveyor;
	[SerializeField] SpriteRenderer m_LowerCornerConveyor;
	[SerializeField] SpriteRenderer m_EndStraightConveyor;

	[SerializeField] float m_ConveyorDefaultSpeed = 0.5f;
	[SerializeField] float m_ConveyorSpeed1 = 1f;
	[SerializeField] float m_ConveyorSpeed2 = 2f;

	ConveyorSocket m_StartSocket, m_EndSocket;
	MachineInstance m_StartSocketMachineScript, m_EndSocketMachineScript;

	float m_CurrentConveyorSpeed;

	public void Initialize(ConveyorSocket _start, ConveyorSocket _end)
	{
		if(ItemDatabase.Instance.DoesItemIDExistInResearch("conveyor-speed-1"))
		{
			if (ItemDatabase.Instance.DoesItemIDExistInResearch("conveyor-speed-2"))
			{
				Debug.Log("Conveyor Speed 2");
				m_CurrentConveyorSpeed = m_ConveyorSpeed2;
			}
			else
			{
				Debug.Log("Conveyor Speed 1");
				m_CurrentConveyorSpeed = m_ConveyorSpeed1;
			}
		}
		else
		{
			Debug.Log("Default Conveyor Speed");
			m_CurrentConveyorSpeed = m_ConveyorDefaultSpeed;
		}

		m_StartSocket = _start;
		m_EndSocket = _end;

		m_StartSocket.SetSocketConnected(true);
		m_EndSocket.SetSocketConnected(true);

		m_StartSocketMachineScript = m_StartSocket.GetMachineScript();
		if(!m_EndSocket.IsBunkerSocket)
		{
			m_EndSocketMachineScript = m_EndSocket.GetMachineScript();
			StartCoroutine(ConveyorWork());
		}
		else StartCoroutine(ConveyorWorkBunker());
	}
	IEnumerator ConveyorWork()
	{
		while (true) 
		{
			yield return new WaitForSeconds(1 / m_CurrentConveyorSpeed);

			InventorySlot[] _inputSlot = m_StartSocketMachineScript.GetOutputSlots();
			InventorySlot[] _outputSlot = m_EndSocketMachineScript.GetInputSlots();

			if (_inputSlot[0] == null) continue;
			if (_inputSlot[0].GetItemAmount() <= 0) continue;
			if (_outputSlot[0].GetItem() != null && _outputSlot[0].GetItem() != _inputSlot[0].GetItem()) continue;
			if (_outputSlot[0].GetItem() != null && _outputSlot[0].GetItemAmount() >= _outputSlot[0].GetItem().StackableAmount) continue;

			if(_outputSlot[0].GetItem() == null) _outputSlot[0].SetItemSlot(_inputSlot[0].GetItem(), 1);
			else _outputSlot[0].AddItemAmount(1);

			_inputSlot[0].AddItemAmount(-1);
		}
	}
	IEnumerator ConveyorWorkBunker()
	{
		while (true)
		{
			yield return new WaitForSeconds(1 / m_CurrentConveyorSpeed);

			InventorySlot[] _inputSlot = m_StartSocketMachineScript.GetOutputSlots();

			if (_inputSlot[0] == null) continue;
			if (_inputSlot[0].GetItemAmount() <= 0) continue;

			if (Bunker.Instance.AddItem(_inputSlot[0].GetItem())) _inputSlot[0].AddItemAmount(-1);
		}
	}
	public void SetPreview(List<Vector2> _points)
	{
		m_StartStraightConveyor.gameObject.SetActive(false);
		m_UpperCornerConveyor.gameObject.SetActive(false);
		m_MidStraightConveyor.gameObject.SetActive(false);
		m_LowerCornerConveyor.gameObject.SetActive(false);
		m_EndStraightConveyor.gameObject.SetActive(false);

		if (_points.Count == 2)
		{
			if (Mathf.Abs(_points[0].x - _points[1].x) < Constant.CONVEYOR_ALIGNMENT_THRESHOLD)
			{
				m_MidStraightConveyor.gameObject.SetActive(true);
				m_MidStraightConveyor.size = new Vector2(Mathf.Abs(_points[1].y - _points[0].y), 1);
				m_MidStraightConveyor.transform.position = (_points[0] + _points[1]) / 2;
			}
			else if (Mathf.Abs(_points[0].y - _points[1].y) < Constant.CONVEYOR_ALIGNMENT_THRESHOLD)
			{
				m_StartStraightConveyor.gameObject.SetActive(true);
				m_StartStraightConveyor.size = new Vector2(Mathf.Abs(_points[1].x - _points[0].x), 1);
				m_StartStraightConveyor.transform.position = (_points[0] + _points[1]) / 2;
			}
			else
			{
				Debug.LogWarning($"Conveyor given 2 points are not correct, Points: {_points[0]}, {_points[1]}");
			}
				
		}
		else if(_points.Count == 4)
		{
			m_StartStraightConveyor.gameObject.SetActive(true);
			m_UpperCornerConveyor.gameObject.SetActive(true);
			m_MidStraightConveyor.gameObject.SetActive(true);
			m_LowerCornerConveyor.gameObject.SetActive(true);
			m_EndStraightConveyor.gameObject.SetActive(true);

			m_StartStraightConveyor.size = new Vector2(Mathf.Abs(_points[1].x - _points[0].x), 1);
			m_StartStraightConveyor.transform.position = (_points[0] + _points[1]) / 2;

			m_UpperCornerConveyor.flipX = _points[1].x - _points[0].x < 0;
			m_UpperCornerConveyor.flipY = _points[2].y - _points[1].y >= 0;
			m_UpperCornerConveyor.transform.position = _points[1];

			m_MidStraightConveyor.size = new Vector2(Mathf.Abs(_points[2].y - _points[1].y), 1);
			m_MidStraightConveyor.transform.position = (_points[1] + _points[2]) / 2;

			m_LowerCornerConveyor.flipX = _points[3].x - _points[2].x >= 0;
			m_LowerCornerConveyor.flipY = _points[2].y - _points[1].y < 0;
			m_LowerCornerConveyor.transform.position = _points[2];

			m_EndStraightConveyor.size = new Vector2(Mathf.Abs(_points[3].x - _points[2].x), 1);
			m_EndStraightConveyor.transform.position = (_points[2] + _points[3]) / 2;
		}
		else
		{
			Debug.LogWarning($"Conveyor points length should be either 2 or 4, but its {_points.Count}");
		}
	}
}
