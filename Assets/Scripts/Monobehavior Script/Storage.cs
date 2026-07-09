using System.Collections;
using UnityEngine;

public class Storage : MonoBehaviour
{
    #region приватные_поля
    [SerializeField] private int _capacity;
    [SerializeField] private ItemData _item;
    [SerializeField] private Transform[] _storageSlots;
    [SerializeField] private GameObject _fruitPrefab;
    private GameObject[] _storagedFruits;
    private int _currentAmount;
    #endregion
    #region добавить
    public bool CanAdd(ItemData item) //сделать enum
    {
        if (item.Type != _item.Type)
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
    private IEnumerator AddCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Фрукт на полку");
        Debug.Log($"{_currentAmount}");
        Debug.Log($"{_storageSlots.Length}");
        Debug.Log(_storagedFruits == null);
        _storagedFruits[_currentAmount] = Instantiate(_fruitPrefab, _storageSlots[_currentAmount].position, Quaternion.identity);
        ++_currentAmount;
    }
    private void Start()
    {
        _storagedFruits = new GameObject[_storageSlots.Length];
    }
    #endregion
    public ItemData RemoveItem()
    {
        if (_currentAmount <= 0)
        {
            return null;
        }
        Destroy(_storagedFruits[_currentAmount - 1]);
        --_currentAmount;
        return _item;
    }
}

