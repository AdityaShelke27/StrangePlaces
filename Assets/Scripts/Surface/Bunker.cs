using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bunker : MonoBehaviour, IActivate
{
	public static Bunker Instance;

	[SerializeField] int m_StorageSaveFrequency;
	[SerializeField] ItemSlot[] m_StorageSlots;
	bool m_IsActivated = false;
	Animator m_Animator;

	int m_CurrentStorageSaveCounter = 0;

	private void Awake()
	{
		if(Instance == null) Instance = this;
	}

	private void Start()
	{
		m_Animator = GetComponent<Animator>();
		AssignStorageSlots();
	}
	private void OnMouseDown()
	{
		SurfaceMovement.s_Selected?.Invoke(gameObject);
	}
	void MovePlayerToBunker()
	{
		InventorySlot[] _inventory = ResourceHandler.Instance.GetInventorySlots();

		for (int i = 0; i < _inventory.Length; i++)
		{
			InventorySlot _inv = _inventory[i];
			PlayerData.itemSlot[i] = new(_inv.GetItem(), _inv.GetItemAmount());
		}

		PlayerData.electricity = PlayerStatsManager.Instance.GetElectricity();
		PlayerData.hunger = PlayerStatsManager.Instance.GetHunger();
		PlayerData.researchPoints = PlayerStatsManager.Instance.GetResearchPoints();

		PlayerData.SaveData();
		SaveStorage();
		SceneManager.LoadScene(Constant.SCENE_BUNKER);
	}

	public void Activate()
	{
		if (m_IsActivated) return;
		m_IsActivated = true;

		StartCoroutine(ActivateTime());
	}
	IEnumerator ActivateTime()
	{
		m_Animator.SetTrigger(Constant.BUNKER_OPEN);

		yield return new WaitForSeconds(1);

		MovePlayerToBunker();
	}

	void AssignStorageSlots()
	{
		if(!ItemDatabase.Instance.DoesItemIDExistInResearch("automation")) return;

		Save_ItemSlotArray[] _itemsArray = Save_Inventory.LoadData();

		m_StorageSlots = new ItemSlot[Constant.STORAGE_SLOT_COUNT];
		if(_itemsArray == null)
		{
			for(int i = 0; i < Constant.STORAGE_SLOT_COUNT; i++)
			{
				m_StorageSlots[i] = new ItemSlot();
			}

			return;
		}

		for(int i = 0; i < _itemsArray.Length; i++)
		{
			m_StorageSlots[i] = new ItemSlot(ItemDatabase.Instance.GetItemByID(_itemsArray[i].id) as StorableItem, _itemsArray[i].amount);
		}
	}

	public bool AddItem(StorableItem _item)
	{
		for(int i = 0; i < m_StorageSlots.Length; i++)
		{
			if(m_StorageSlots[i].item == null)
			{
				m_StorageSlots[i].item = _item;
				m_StorageSlots[i].amount = 1;

				IncrementStorageSaveCounter();
				return true;
			}
			else if (m_StorageSlots[i].item == _item && m_StorageSlots[i].amount < _item.StackableAmount)
			{
				m_StorageSlots[i].amount++;

				IncrementStorageSaveCounter();
				return true;
			}
		}

		return false;
	}
	void IncrementStorageSaveCounter()
	{
		m_CurrentStorageSaveCounter++;

		if(m_CurrentStorageSaveCounter >= m_StorageSaveFrequency) 
		{ 
			SaveStorage();
			m_CurrentStorageSaveCounter = 0;
		}
	}
	void SaveStorage()
	{
		Save_Inventory _saveInv = new()
		{
			itemSlot = new Save_ItemSlotArray[m_StorageSlots.Length]
		};

		for (int i = 0; i < m_StorageSlots.Length; i++)
		{
			StorableItem _item = m_StorageSlots[i].item;
			_saveInv.itemSlot[i] = _item != null ? new(_item.itemID, m_StorageSlots[i].amount) : new("", 0);
		}

		Save_Inventory.SaveData(_saveInv);
	}
}
