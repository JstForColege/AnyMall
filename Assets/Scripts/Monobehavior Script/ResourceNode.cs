using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceNode : MonoBehaviour
{
    #region приватные_поля
    [SerializeField]
    private int _amount;
    [SerializeField]
    private int _timer;

    private int _currentAmount = 0;
    [SerializeField]
    private ItemData _food;

    [SerializeField]
    private Transform[] _fruitSpawns;
    private GameObject[] _spawnedFruits;
    [SerializeField]
    private GameObject _fruitPrefab;
    [SerializeField]
    private ItemData _itemData;

    private bool _isGrowing = false;
    #endregion
    #region рост
    private void Grow()
    {
        _spawnedFruits = new GameObject[_fruitSpawns.Length];
        if (!_isGrowing)
        {
            _isGrowing = true;
            StartCoroutine(GrowCoroutine());
        }
    }
    private IEnumerator GrowCoroutine()
    {
        while (_currentAmount < _amount)
        {
            yield return new WaitForSeconds(_timer);
            ++_currentAmount;
            _spawnedFruits[_currentAmount - 1] = Instantiate(_fruitPrefab, _fruitSpawns[_currentAmount - 1].position, Quaternion.identity);
        }
        _isGrowing = false;
    }
    private void Start()
    {
        Grow();
    }
    #endregion
    public ItemData Harvest()
    {
        if (_currentAmount <= 0)
        {
            Debug.Log("Нет плодов для сбора");
            return null;
        }
        --_currentAmount;
        Destroy(_spawnedFruits[_currentAmount]);
        _spawnedFruits[_currentAmount] = null;
        return _itemData;
    }
}


public class ResourceFood //Заглушка покаа Юсуф не сделает класс
{
    public int id;
}