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
    public ItemType GetItemType()
    {
        return _item.Type;
    }
    public bool AddItem(ItemData item)
    {
        if (!CanAdd(item))
        {
            return false;
        }
        int index = _currentAmount;
        ++_currentAmount;
        StartCoroutine(AddCoroutine(index));
        return true;
    }
    private IEnumerator AddCoroutine(int index)
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Фрукт на полку");
        Debug.Log($"{_currentAmount}");
        Debug.Log($"{_storageSlots.Length}");
        Debug.Log(_storagedFruits == null);
        _storagedFruits[index] = Instantiate(_fruitPrefab, _storageSlots[index].position, Quaternion.identity);
    }
    private void Start()
    {
        _storagedFruits = new GameObject[_capacity];
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

