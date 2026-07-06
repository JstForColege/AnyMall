using System.Collections;
using UnityEngine;

public class Producer : MonoBehaviour
{
    #region приватные_поля
    private ResourceFood _ingridient;
    private int _maxIngridient;
    private int _currentIngridient = 0;

    private ResourceFood _outgridient;
    private int _maxOutgridient;
    private int _currentOutgridient = 0;

    private int _makingTime;
    private bool _isWorking = true;
    private string _name;
    #endregion
    #region публичные_свойства
    public ResourceFood Ingridient
    {
        get => _ingridient;
        set => _ingridient = value;
    }
    public int MaxIngridient
    {
        get => _maxIngridient;
        set => _maxIngridient = value;
    }
    public int CurrentIngridient
    {
        get => _currentIngridient;
        set => _currentIngridient = value;
    }

    public ResourceFood Outgridient
    {
        get => _outgridient;
        set => _outgridient = value;
    }
    public int MaxOutgridient
    {
        get => _maxOutgridient;
        set => _maxOutgridient = value;
    }
    public int CurrentOutgridient
    {
        get => _currentOutgridient;
        set => _currentIngridient = value;
    }

    public int MakingTime
    {
        get => _makingTime;
        set => _makingTime = value;
    }
    public bool IsWorking
    {
        get => _isWorking;
        set => _isWorking = value;
    }
    public string Name
    {
        get => _name;
        set => _name = value;
    }
    #endregion


    public void Produce()
    {
        if (IsWorking == true)
        {
            StartCoroutine(ProduceCoroutine());
        }
        else
        {
            Debug.Log($"{Name} не работает!");
        }
    }
    public IEnumerator ProduceCoroutine()
    {
        while (CurrentOutgridient < MaxOutgridient && CurrentIngridient > 0)
        {
            --CurrentIngridient;
            yield return new WaitForSeconds(MakingTime);
            ++CurrentOutgridient;
        }
    }
}