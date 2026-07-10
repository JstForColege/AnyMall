using System.Collections;
using UnityEngine;

public enum ProducerType { Animal, Machine }

public class Producer : MonoBehaviour
{
    #region приватные_поля
    [SerializeField] private GameObject _inPrefab;
    [SerializeField] private ItemData _ingridient;
    [SerializeField] private int _maxIngridient;
    [SerializeField] private Transform[] _ingridientSlots;
    private GameObject[] _addedIngridients;
    private int _currentIngridient = 0;

    [SerializeField] private GameObject _outPrefab;
    [SerializeField] private ItemData _outgridient;
    [SerializeField] private int _maxOutgridient;
    [SerializeField] private Transform[] _outgridientSlots;
    private GameObject[] _producedOutgridients;
    private int _currentOutgridient = 0;

    [SerializeField] private int _makingTime;
    private bool _isWorking = false;

    [SerializeField] private ProducerType _producerType;
    private int _baseMaxIngridient;
    private int _baseMaxOutgridient;
    private float _baseMakingTime;
    #endregion

    public bool CanAdd(ItemData item)
    {
        if (item.Type != _ingridient.Type) return false;
        if (_currentIngridient >= _maxIngridient) return false;
        return true;
    }

    public bool AddItem(ItemData item)
    {
        if (!CanAdd(item)) return false;
        ++_currentIngridient;
        StartCoroutine(AddCoroutine(_currentIngridient - 1));
        return true;
    }

    private IEnumerator AddCoroutine(int index)
    {
        yield return new WaitForSeconds(0.3f);
        _addedIngridients[index] = Instantiate(_inPrefab, _ingridientSlots[index].position, Quaternion.identity);
        Produce();
    }

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
            _producedOutgridients[_currentOutgridient] = Instantiate(_outPrefab, _outgridientSlots[_currentOutgridient].position, Quaternion.identity);
            ++_currentOutgridient;
        }
        _isWorking = false;
        if (_currentIngridient > 0 && _currentOutgridient < _maxOutgridient) Produce();
    }

    public ItemData TakeOutgridient()
    {
        if (_currentOutgridient <= 0) return null;
        --_currentOutgridient;
        Destroy(_producedOutgridients[_currentOutgridient]);
        _producedOutgridients[_currentOutgridient] = null;
        Produce();
        return _outgridient;
    }

    private void Awake()
    {
        _baseMaxIngridient = _maxIngridient;
        _baseMaxOutgridient = _maxOutgridient;
        _baseMakingTime = _makingTime;
    }

    private void Start()
    {
        _addedIngridients = new GameObject[_maxIngridient];
        _producedOutgridients = new GameObject[_maxOutgridient];
        Produce();

        StartCoroutine(InitializeUpgrades());
    }

    private IEnumerator InitializeUpgrades()
    {
        yield return null;

        if (UpgradeSystem.Instance == null)
        {
            Debug.LogWarning($"[Producer] {name}: UpgradeSystem не найден, улучшения не будут работать.");
            yield break;
        }

        if (_producerType == ProducerType.Animal)
        {
            UpgradeSystem.Instance.Subscribe(UpgradeType.ANIMAL_FEED, OnUpgradeChanged);
            UpgradeSystem.Instance.Subscribe(UpgradeType.ANIMAL_SPEED, OnUpgradeChanged);
            UpgradeSystem.Instance.Subscribe(UpgradeType.ANIMAL_STAMINA, OnUpgradeChanged);
        }
        else if (_producerType == ProducerType.Machine)
        {
            UpgradeSystem.Instance.Subscribe(UpgradeType.MACHINE_CAPACITY, OnUpgradeChanged);
            UpgradeSystem.Instance.Subscribe(UpgradeType.MACHINE_SPEED, OnUpgradeChanged);
            UpgradeSystem.Instance.Subscribe(UpgradeType.MACHINE_DURABILITY, OnUpgradeChanged);
        }

        ApplyUpgrades();
        Debug.Log($"[Producer] {name} улучшения инициализированы.");
    }

    private void OnUpgradeChanged(int newLevel)
    {
        ApplyUpgrades();
    }

    private void ApplyUpgrades()
    {
        if (UpgradeSystem.Instance == null) return;

        int capacityLevel = 0, speedLevel = 0, staminaOrDurabilityLevel = 0;
        if (_producerType == ProducerType.Animal)
        {
            capacityLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.ANIMAL_FEED);
            speedLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.ANIMAL_SPEED);
            staminaOrDurabilityLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.ANIMAL_STAMINA);
        }
        else if (_producerType == ProducerType.Machine)
        {
            capacityLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.MACHINE_CAPACITY);
            speedLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.MACHINE_SPEED);
            staminaOrDurabilityLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.MACHINE_DURABILITY);
        }

        int newMaxIngridient = _baseMaxIngridient + capacityLevel * 2;
        int newMaxOutgridient = _baseMaxOutgridient + capacityLevel * 2;
        int newMakingTime = Mathf.Max(1, (int)(_baseMakingTime / (1f + speedLevel * 0.2f)));

        _maxIngridient = newMaxIngridient;
        _maxOutgridient = newMaxOutgridient;
        _makingTime = newMakingTime;

        if (_addedIngridients != null && _addedIngridients.Length < _maxIngridient)
        {
            GameObject[] newAdded = new GameObject[_maxIngridient];
            int copyCount = Mathf.Min(_addedIngridients.Length, _maxIngridient);
            for (int i = 0; i < copyCount; i++)
                newAdded[i] = _addedIngridients[i];
            _addedIngridients = newAdded;
        }

        if (_producedOutgridients != null && _producedOutgridients.Length < _maxOutgridient)
        {
            GameObject[] newProduced = new GameObject[_maxOutgridient];
            int copyCount = Mathf.Min(_producedOutgridients.Length, _maxOutgridient);
            for (int i = 0; i < copyCount; i++)
                newProduced[i] = _producedOutgridients[i];
            _producedOutgridients = newProduced;
        }

        Debug.Log($"[Producer] {name} обновлён: ёмкость={_maxIngridient}, время={_makingTime}с, стамина={staminaOrDurabilityLevel}");
    }

    private void OnDestroy()
    {
        if (UpgradeSystem.Instance != null)
        {
            if (_producerType == ProducerType.Animal)
            {
                UpgradeSystem.Instance.Unsubscribe(UpgradeType.ANIMAL_FEED, OnUpgradeChanged);
                UpgradeSystem.Instance.Unsubscribe(UpgradeType.ANIMAL_SPEED, OnUpgradeChanged);
                UpgradeSystem.Instance.Unsubscribe(UpgradeType.ANIMAL_STAMINA, OnUpgradeChanged);
            }
            else if (_producerType == ProducerType.Machine)
            {
                UpgradeSystem.Instance.Unsubscribe(UpgradeType.MACHINE_CAPACITY, OnUpgradeChanged);
                UpgradeSystem.Instance.Unsubscribe(UpgradeType.MACHINE_SPEED, OnUpgradeChanged);
                UpgradeSystem.Instance.Unsubscribe(UpgradeType.MACHINE_DURABILITY, OnUpgradeChanged);
            }
        }
    }
}