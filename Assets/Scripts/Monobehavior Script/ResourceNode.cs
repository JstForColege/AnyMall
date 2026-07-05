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
    //private ResourceFood _food;

    [SerializeField]
    private Transform[] _fruitSpawns;
    [SerializeField]
    private GameObject _fruitPrefab;

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
    /*public ResourceFood Food
    {
        get => _food;
        set => _food = value;
    }*/
    #endregion

    private void Grow()
    {
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
            Instantiate(_fruitPrefab, _fruitSpawns[CurrentAmount - 1].position, Quaternion.identity);
        }
        _isGrowing = false;
    }

    private void Start()
    {
        Grow();
    }
}


public class ResourceFood //Заглушка покаа Юсуф не сделает класс
{
    public int id;
}