using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _upgradeButton;

    private IWalletAccess _wallet;
    private UpgradeType _type;

    public void Initialize(UpgradeType type)
    {
        _type = type;
        UpdateUI();
    }

    public void UpdateUI()
    {
        int level = UpgradeSystem.Instance.GetCurrentLevel(_type);
        int cost = UpgradeSystem.Instance.GetNextLevelCost(_type);
        bool isMax = UpgradeSystem.Instance.IsMaxLevel(_type);
        bool canAfford = UpgradeSystem.Instance.CanUpgrade(_type);

        _levelText.text = $"Уровень: {level}";
        _costText.text = isMax ? "MAX" : $"{cost} $";
        _upgradeButton.interactable = canAfford;

        _nameText.text = GetDisplayName(_type);
    }

    public void OnUpgradeButtonClick()
    {
        if (UpgradeSystem.Instance.TryUpgrade(_type))
        {
            UpdateUI();

            UpgradeWindow window = FindFirstObjectByType<UpgradeWindow>();
            if (window != null)
            {
                window.RefreshAllItems();
            }
        }
        else
        {
            Debug.Log("Недостаточно денег!");
        }
    }

    private string GetDisplayName(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.PLAYER_INVENTORY: return "Вместимость инвентаря";
            case UpgradeType.ANIMAL_FEED: return "Кормушка (животные)";
            case UpgradeType.ANIMAL_SPEED: return "Скорость животных";
            case UpgradeType.ANIMAL_STAMINA: return "Выносливость животных";
            case UpgradeType.MACHINE_CAPACITY: return "Ёмкость техники";
            case UpgradeType.MACHINE_SPEED: return "Скорость техники";
            case UpgradeType.MACHINE_DURABILITY: return "Надёжность техники";
            default: return type.ToString();
        }
    }
}