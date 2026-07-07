using System;
using System.Collections;
using UnityEngine;

public class Storage : MonoBehaviour
{
	[SerializeField]
	private int _capacity;
	[SerializeField]
	private ItemData _item;
	[SerializeField]
	private Transform[] _storageSlots;
    [SerializeField]
    private GameObject _fruitPrefab;
    private int _currentAmount;
	public int CurrentAmount
	{
		get => _currentAmount;
		set => _currentAmount = value;
	}
	public bool CanAdd(ItemData item)
	{
		if (item != _item)
		{
			return false;
		}
		if (_currentAmount >= _capacity)
		{ 
			return false;
		}
			
		return true;
	}
	public bool AddItem(ItemData item)
	{
		if (!CanAdd(item))
		{
			return false;
		}
		StartCoroutine(AddCoroutine());
		return true;
	}
	public ItemData RemoveItem()
	{
		if (_currentAmount <= 0)
		{
			return null;
		}
		-- _currentAmount;
		return _item;
	}

	public IEnumerator AddCoroutine()
	{
        yield return new WaitForSeconds(1);
		++CurrentAmount;
		Debug.Log("Фрукт на полкку");
		Instantiate(_fruitPrefab, _storageSlots[_currentAmount].position, Quaternion.identity);
    }
}

