using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    #region приватные_поля
    private int _amount;
    private int _timer;
    private int _currentAmount = 0;
    private ResourceFood _food;
    private bool _isGrowing = false;
    #endregion
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
    public ResourceFood Food
    {
        get => _food;
        set => _food = value;
    }

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