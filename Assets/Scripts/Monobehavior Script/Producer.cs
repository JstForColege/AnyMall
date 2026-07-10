using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEditor.Progress;

public class Producer : MonoBehaviour
{
    #region приватные_поля
    #region ingridient
    [SerializeField] private GameObject _inPrefab;
    [SerializeField] private ItemData _ingridient;
    [SerializeField] private int _maxIngridient;
    [SerializeField] private Transform[] _ingridientSlots;
    private GameObject[] _addedIngridients;
    private int _currentIngridient = 0;
    #endregion
    #region outgridient
    [SerializeField] private GameObject _outPrefab;
    [SerializeField] private ItemData _outgridient;
    [SerializeField] private int _maxOutgridient;
    [SerializeField] private Transform[] _outgridientSlots;
    private GameObject[] _producedOutgridients;
    private int _currentOutgridient = 0;
    #endregion
    [SerializeField] private int _makingTime;
    private bool _isWorking = false;
    #endregion
    #region Добавить
    public bool CanAdd(ItemData item)
    {
        if (item.Type != _ingridient.Type)
        {
            return false;
        }
        if (_currentIngridient >= _maxIngridient)
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
        int index = _currentIngridient;
        ++_currentIngridient;

        yield return new WaitForSeconds(0.3f);
        Debug.Log("ингридиент на полку");
        _addedIngridients[index] = 
            Instantiate(_inPrefab, _ingridientSlots[index].position, Quaternion.identity);
        ++_currentIngridient;
        Produce();
    }
    #endregion
    #region Произвести
    public void Produce()
    {
        if (!_isWorking)
        {
            _isWorking = true;
            StartCoroutine(ProduceCoroutine());
        }
    }
    public IEnumerator ProduceCoroutine()
    {
        while (_currentOutgridient < _maxOutgridient && _currentIngridient > 0)
        {
            --_currentIngridient;
            Destroy(_addedIngridients[_currentIngridient]);
            _addedIngridients[_currentIngridient] = null;
            yield return new WaitForSeconds(_makingTime);
            ++_currentOutgridient;
            _producedOutgridients[_currentOutgridient] =
                Instantiate(_outPrefab, _outgridientSlots[_currentOutgridient].position, Quaternion.identity);
        }
        _isWorking = false;

        if (_currentIngridient > 0 && _currentOutgridient < _maxOutgridient) Produce();
    }
    #endregion
    
    public ItemData TakeOutgridient()
    {
        if (_currentOutgridient <= 0) 
            return null;
        -- _currentOutgridient;
        Destroy(_producedOutgridients[_currentOutgridient]);
        _producedOutgridients[_currentOutgridient] = null;

        Produce();
        return _outgridient;
    }
    private void Start()
    {
        _addedIngridients = new GameObject[_maxIngridient];
        _producedOutgridients = new GameObject[_maxOutgridient];

        Produce();
    }
}