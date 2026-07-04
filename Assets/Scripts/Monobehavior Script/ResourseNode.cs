using System;
using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    private int _amount;
    private int _timer;
    private int _currentAmount = 0;
    private ResourceFood _food;

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

    public void Grow()
    {
        if (CurrentAmount <= Amount)
        {
            StartCoroutine(GrowCoroutine());
        }
    }

    public IEnumerator GrowCoroutine()
    {
        yield return new WaitForSeconds(Timer);
        ++CurrentAmount;
    }
}


public class ResourceFood //Заглушка покаа Юсуф не сделает класс
{
    public int id;
}