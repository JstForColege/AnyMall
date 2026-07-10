using System;
using System.Collections.Generic;
using UnityEngine;


//                              ГАЙД ПО АПГРЕЙДАМ (Код ниже)
// 
// 1. КАК СОХРАНЯЮТСЯ УРОВНИ
//     Уровни автоматически сохраняются в SaveSystem при каждом улучшении
//     (вызов TryUpgrade) под ключом "upgrades"
//     При старте игры уровни загружаются автоматически в Awake()
//     Вам не нужно вручную вызывать SaveObject/LoadObject для UpgradeSystem
// 
// 2. КАК ИСПОЛЬЗОВАТЬ В UI
//     Для отображения кнопок улучшения используй методы:
//        GetCurrentLevel(UpgradeType type) - текущий уровень
//        GetNextLevelCost(UpgradeType type) - цена следующего уровня (или int.MaxValue, если максимум)
//        IsMaxLevel(UpgradeType type) - достигнут ли максимум
//        CanUpgrade(UpgradeType type) - можно ли улучшить (есть деньги и не максимум)
//     При нажатии на кнопку улучшения вызывай TryUpgrade(type) - он вернёт true, если успешно
//     После успеха обнови UI (цены и уровни могут измениться)
// 
// 3. ПРИМЕР ИСПОЛЬЗОВАНИЯ В UI
//    public void UpdateUpgradeUI()
//    {
//        int level = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
//        int cost = UpgradeSystem.Instance.GetNextLevelCost(UpgradeType.PLAYER_INVENTORY);
//        bool max = UpgradeSystem.Instance.IsMaxLevel(UpgradeType.PLAYER_INVENTORY);
//        bool canAfford = UpgradeSystem.Instance.CanUpgrade(UpgradeType.PLAYER_INVENTORY);
//    
//        levelText.text = level.ToString();
//        costText.text = max ? "MAX" : cost.ToString();
//        upgradeButton.interactable = canAfford;
//    }
// 
//    public void OnUpgradeButtonClick()
//    {
//        if (UpgradeSystem.Instance.TryUpgrade(UpgradeType.PLAYER_INVENTORY))
//        {
//            UpdateUpgradeUI(); // обновить интерфейс
//        }
//    }
// 
// 4. КАК ЭФФЕКТЫ УЛУЧШЕНИЙ ПРИМЕНЯЮТСЯ К ДРУГИМ СИСТЕМАМ
//     UpgradeSystem НЕ применяет эффекты автоматически. Он только хранит уровни
//     Другие системы (PlayerController, WorldObjects) должны сами проверять уровни
//     и применять эффекты при необходимости
//     Например, PlayerController в Start() может сделать:
//       int inventoryLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
//       _maxInventorySize = 1 + inventoryLevel; // базовый размер 1, каждый уровень +1
//     Животные и техника могут запрашивать свои уровни при старте или при изменении
// 
// 5. ДОСТУПНЫЕ ТИПЫ УЛУЧШЕНИЙ
//    - PLAYER_INVENTORY   - вместимость инвентаря игрока
//    - ANIMAL_FEED        - количество еды в кормушке
//    - ANIMAL_SPEED       - скорость производства животных
//    - ANIMAL_STAMINA     - время до сна животных
//    - MACHINE_CAPACITY   - количество ингредиентов в технике
//    - MACHINE_SPEED      - скорость производства техники
//    - MACHINE_DURABILITY - время до поломки техники
// 
// 6. ИНИЦИАЛИЗАЦИЯ IWalletAccess
//     В Bootstrap или в Start() игрока:
//       var wallet = FindObjectOfType<PlayerWallet>(); // или получить через интерфейс
//       UpgradeSystem.Instance.Initialize(wallet);
//     Без вызова Initialize все методы, требующие денег (CanUpgrade, TryUpgrade), вернут false
// 
// 7. ПРИМЕР ПОЛНОГО ЦИКЛА (без событий)
//    private void Start()
//    {
//        // Получаем уровень прокачки и применяем эффект
//        int level = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
//        _maxInventorySize = 1 + level; // базовый размер 1
//    }
// 
//    // При изменении уровня (например, через UI) обновляем эффект
//    private void OnUpgradeApplied()
//    {
//        int newLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
//        _maxInventorySize = 1 + newLevel;
//        // также обновляем UI инвентаря, если нужно
//    }
//
// 8. КАК УЗНАТЬ ОБ ИЗМЕНЕНИИ КОНКРЕТНОГО УЛУЧШЕНИЯ (СОБЫТИЯ ДЛЯ КАЖДОГО ТИПА)
//     Вместо одного общего события, в UpgradeSystem реализован словарь событий,
//     где каждое событие соответствует своему типу улучшения
//     Это позволяет подписываться ТОЛЬКО на те улучшения, которые нужны вашей системе,
//     что повышает производительность и снижает количество лишних проверок
//
//     Чтобы подписаться на событие для конкретного типа, используйте метод Subscribe:
//       UpgradeSystem.Instance.Subscribe(UpgradeType.PLAYER_INVENTORY, OnInventoryUpgraded);
//
//     Пример подписки в PlayerController:
//       private void Start()
//       {
//           UpgradeSystem.Instance.Subscribe(UpgradeType.PLAYER_INVENTORY, OnInventoryUpgraded);
//           // Применяем текущий уровень при старте
//           int level = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
//           OnInventoryUpgraded(level);
//       }
//
//     Пример обработки события:
//       private void OnInventoryUpgraded(int newLevel)
//       {
//           _maxInventorySize = 3 + newLevel; // базовый размер 1, каждый уровень +1
//           Debug.Log($"Inventory size updated to {_maxInventorySize}");
//           // Обновляем UI, если нужно
//       }
//
//     Для отписки используйте Unsubscribe:
//       private void OnDestroy()
//       {
//           UpgradeSystem.Instance.Unsubscribe(UpgradeType.PLAYER_INVENTORY, OnInventoryUpgraded);
//       }
//
// 9. ПРИМЕР ДЛЯ WORLD OBJECTS (ЖИВОТНЫЕ И ТЕХНИКА)
//     Животные и техника должны подписываться на свои типы улучшений
//     Базовый класс для всех производителей:
//       public abstract class BaseProducer : MonoBehaviour
//       {
//           protected UpgradeType _speedType;
//           protected UpgradeType _capacityType;
//           protected UpgradeType _staminaOrDurabilityType;
//
//           protected virtual void Start()
//           {
//               UpgradeSystem.Instance.Subscribe(_speedType, OnUpgradeChanged);
//               UpgradeSystem.Instance.Subscribe(_capacityType, OnUpgradeChanged);
//               UpgradeSystem.Instance.Subscribe(_staminaOrDurabilityType, OnUpgradeChanged);
//               ApplyUpgrades(); // применяем текущие уровни
//           }
//
//           private void OnUpgradeChanged(int newLevel)
//           {
//               ApplyUpgrades();
//           }
//
//           protected virtual void ApplyUpgrades()
//           {
//               // Переопределяется в каждом классе
//           }
//
//           protected virtual void OnDestroy()
//           {
//               UpgradeSystem.Instance.Unsubscribe(_speedType, OnUpgradeChanged);
//               UpgradeSystem.Instance.Unsubscribe(_capacityType, OnUpgradeChanged);
//               UpgradeSystem.Instance.Unsubscribe(_staminaOrDurabilityType, OnUpgradeChanged);
//           }
//       }
//
//     Пример для курицы:
//       public class Chicken : BaseProducer
//       {
//           private void Awake()
//           {
//               _speedType = UpgradeType.ANIMAL_SPEED;
//               _capacityType = UpgradeType.ANIMAL_FEED;
//               _staminaOrDurabilityType = UpgradeType.ANIMAL_STAMINA;
//           }
//
//           protected override void ApplyUpgrades()
//           {
//               int speedLevel = UpgradeSystem.Instance.GetCurrentLevel(_speedType);
//               int feedLevel = UpgradeSystem.Instance.GetCurrentLevel(_capacityType);
//               int staminaLevel = UpgradeSystem.Instance.GetCurrentLevel(_staminaOrDurabilityType);
//               
//               _productionTime = 5f / (1f + speedLevel * 0.2f);
//               _maxFeed = 3 + feedLevel * 2;
//               _sleepTime = 60f + staminaLevel * 10f;
//           }
//       }
//
// 10. ПРИМЕНЕНИЕ ВСЕХ ЭФФЕКТОВ ПРИ СТАРТЕ (ApplyAllUpgrades)
//     После загрузки сохранённых уровней, UpgradeSystem автоматически вызывает ApplyAllUpgrades(),
//     которая проходится по всем типам и вызывает соответствующие события
//     Это гарантирует, что все системы получат начальные значения
//
// 11. ОТПИСКА ОТ СОБЫТИЙ (ВАЖНО!)
//     При уничтожении объекта обязательно отписывайтесь от событий с помощью Unsubscribe,
//     чтобы избежать утечек памяти и ошибок
//
// 12. ПРОВЕРКА ПОДКЛЮЧЕНИЯ IWalletAccess
//     Если улучшения не работают, проверьте, что UpgradeSystem получил ссылку на кошелёк
//     В PlayerWallet должен быть вызов: UpgradeSystem.Instance.Initialize(this);
//     Можно добавить проверку: public bool IsWalletInitialized => _wallet != null;
//
// 13. ОБНОВЛЕНИЕ UI ПОСЛЕ УЛУЧШЕНИЯ
//     После успешного улучшения (TryUpgrade вернул true) обновите UI
//     В UpgradeItemUI это делается автоматически через UpdateUI()
//     Если у вас есть другие UI-элементы, подпишитесь на соответствующее событие


public class UpgradeSystem : MonoBehaviour
{
    private static UpgradeSystem _instance;
    public static UpgradeSystem Instance => _instance;

    private IWalletAccess _wallet;
    private Dictionary<UpgradeType, int> _currentLevels = new Dictionary<UpgradeType, int>();
    private Dictionary<UpgradeType, List<int>> _costs = new Dictionary<UpgradeType, List<int>>();

    private Dictionary<UpgradeType, Action<int>> _upgradeEvents = new Dictionary<UpgradeType, Action<int>>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeCosts();
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            _upgradeEvents[type] = null;
        }
    }

    private void Start()
    {
        LoadLevels();
        ApplyAllUpgrades();
    }

    public void Subscribe(UpgradeType type, Action<int> callback)
    {
        if (_upgradeEvents.ContainsKey(type))
        {
            _upgradeEvents[type] += callback;
        }
        else
        {
            Debug.LogError($"UpgradeSystem: Unknown upgrade type {type}");
        }
    }

    public void Unsubscribe(UpgradeType type, Action<int> callback)
    {
        if (_upgradeEvents.ContainsKey(type))
        {
            _upgradeEvents[type] -= callback;
        }
    }

    public void Initialize(IWalletAccess wallet)
    {
        _wallet = wallet;
    }

    private void InitializeCosts()
    {
        _costs[UpgradeType.PLAYER_INVENTORY] = new List<int> { 100, 250, 500 };
        _costs[UpgradeType.ANIMAL_FEED] = new List<int> { 150, 300, 550 };
        _costs[UpgradeType.ANIMAL_SPEED] = new List<int> { 200, 400, 700 };
        _costs[UpgradeType.ANIMAL_STAMINA] = new List<int> { 250, 500, 800 };
        _costs[UpgradeType.MACHINE_CAPACITY] = new List<int> { 100, 250, 450 };
        _costs[UpgradeType.MACHINE_SPEED] = new List<int> { 150, 350, 600 };
        _costs[UpgradeType.MACHINE_DURABILITY] = new List<int> { 200, 450, 750 };
    }

    public bool CanUpgrade(UpgradeType type)
    {
        if (_wallet == null) return false;
        if (IsMaxLevel(type)) return false;
        int cost = GetNextLevelCost(type);
        return _wallet.GetMoney() >= cost;
    }

    public bool TryUpgrade(UpgradeType type)
    {
        if (!CanUpgrade(type)) return false;

        int cost = GetNextLevelCost(type);
        if (!_wallet.SpendMoney(cost)) return false;

        _currentLevels[type] = _currentLevels.ContainsKey(type) ? _currentLevels[type] + 1 : 1;
        int newLevel = _currentLevels[type];

        SaveLevels();

        if (_upgradeEvents.ContainsKey(type))
        {
            _upgradeEvents[type]?.Invoke(newLevel);
        }

        Debug.Log($"Upgraded {type} to level {newLevel}");
        return true;
    }

    public int GetNextLevelCost(UpgradeType type)
    {
        if (!_costs.ContainsKey(type)) return int.MaxValue;
        int currentLevel = GetCurrentLevel(type);
        if (currentLevel >= _costs[type].Count) return int.MaxValue;
        return _costs[type][currentLevel];
    }

    public int GetCurrentLevel(UpgradeType type)
    {
        return _currentLevels.ContainsKey(type) ? _currentLevels[type] : 0;
    }

    public bool IsMaxLevel(UpgradeType type)
    {
        if (!_costs.ContainsKey(type)) return true;
        return GetCurrentLevel(type) >= _costs[type].Count;
    }

    public void ApplyAllUpgrades()
    {
        foreach (var type in _currentLevels.Keys)
        {
            int level = _currentLevels[type];
            if (_upgradeEvents.ContainsKey(type))
            {
                _upgradeEvents[type]?.Invoke(level);
            }
        }
    }
    private void SaveLevels()
    {
        SaveSystem.Instance.SaveObject("upgrades", _currentLevels);
        SaveSystem.Instance.MarkDirty();
    }

    private void LoadLevels()
    {
        object raw = SaveSystem.Instance.LoadObject("upgrades");
        if (raw != null)
        {
            _currentLevels = raw as Dictionary<UpgradeType, int>;
            if (_currentLevels == null)
            {
                _currentLevels = new Dictionary<UpgradeType, int>();
            }
        }
        else
        {
            _currentLevels = new Dictionary<UpgradeType, int>();
        }

        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            if (!_currentLevels.ContainsKey(type))
                _currentLevels[type] = 0;
        }
    }

    public void RestoreLevels(Dictionary<UpgradeType, int> levels)
    {
        if (levels != null)
        {
            _currentLevels = levels;
        }
    }
}