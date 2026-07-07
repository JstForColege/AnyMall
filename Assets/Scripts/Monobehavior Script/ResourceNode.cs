using System.Collections;
using UnityEngine;

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
    [SerializeField] private ItemData _itemData;

    private bool _isGrowing = false;
    #endregion
    #region публичные_свойства
    public int Amount
    {
        get => _amount;
        set => _amount = value;
    }
    public int CurrentAmount
    {
        get => _currentAmount;
        set => _currentAmount = value;
    }
    public int Timer
    {
        get => _timer;
        set => _timer = value;
    }
    #endregion

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
        while (CurrentAmount < Amount)
        {
            yield return new WaitForSeconds(Timer);
            ++CurrentAmount;
            _spawnedFruits[CurrentAmount - 1] = Instantiate(_fruitPrefab, _fruitSpawns[CurrentAmount - 1].position, Quaternion.identity);
        }
        _isGrowing = false;
    }

    private void Start()
    {
        Grow();
    }

    public ItemData Harvest()
    {
        if (_currentAmount <= 0)
        {
            Debug.Log("Нет плодов для сбора");
            return null;
        }

        -- _currentAmount;
        Destroy(_spawnedFruits[CurrentAmount]);
        _spawnedFruits[_currentAmount] = null;
        return _itemData;
    }
}


public class ResourceFood //Заглушка покаа Юсуф не сделает класс
{
    public int id;
}