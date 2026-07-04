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
//        GetNextLevelCost(UpgradeType type) - цена следующего уровня (или int.MaxValue, если максимум).
//        IsMaxLevel(UpgradeType type) - достигнут ли максимум
//        CanUpgrade(UpgradeType type) - можно ли улучшить (есть деньги и не максимум).
//     При нажатии на кнопку улучшения вызывай TryUpgrade(type) - он вернёт true, если успешно.
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
// 7. ПРИМЕР ПОЛНОГО ЦИКЛА
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


public class UpgradeSystem : MonoBehaviour
{
    private static UpgradeSystem _instance;
    public static UpgradeSystem Instance => _instance;

    private IWalletAccess _wallet;
    private Dictionary<UpgradeType, int> _currentLevels = new Dictionary<UpgradeType, int>();
    private Dictionary<UpgradeType, List<int>> _costs = new Dictionary<UpgradeType, List<int>>();

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
    }

    private void Start()
    {

        LoadLevels();
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

        if (!_currentLevels.ContainsKey(type))
            _currentLevels[type] = 0;
        _currentLevels[type]++;

        SaveLevels();
        Debug.Log($"Upgraded {type} to level {_currentLevels[type]}");
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
    }

    public void RestoreLevels(Dictionary<UpgradeType, int> levels)
    {
        if (levels != null)
        {
            _currentLevels = levels;
        }
    }
}