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
    public void Produce()
    {
        if (_isWorking == true)
        {
            StartCoroutine(ProduceCoroutine());
        }
        else
        {
            Debug.Log($"{_name} не работает!");
        }
    }
    public IEnumerator ProduceCoroutine()
    {
        while (_currentIngridient < _maxIngridient && _currentIngridient > 0)
        {
            --_currentIngridient;
            yield return new WaitForSeconds(_makingTime);
            ++_currentOutgridient;
        }
    }
}