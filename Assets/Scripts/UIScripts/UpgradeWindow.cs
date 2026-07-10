using System.Collections.Generic;
using UnityEngine;

public class UpgradeWindow : MonoBehaviour
{
    [SerializeField] private Transform _contentParent;
    [SerializeField] private GameObject _upgradeItemPrefab;

    private List<UpgradeItemUI> _items = new List<UpgradeItemUI>();
    public void OnWindowOpen()
    {
        BuildUpgradeList();
    }

    private void BuildUpgradeList()
    {
        if (_items.Count > 0)
        {
            RefreshAllItems();
            return;
        }

        UpgradeType[] upgradeTypes = (UpgradeType[])System.Enum.GetValues(typeof(UpgradeType));

        foreach (UpgradeType type in upgradeTypes)
        {
            GameObject itemObj = Instantiate(_upgradeItemPrefab, _contentParent);

            UpgradeItemUI itemUI = itemObj.GetComponent<UpgradeItemUI>();
            if (itemUI != null)
            {
                itemUI.Initialize(type);
                _items.Add(itemUI);
            }
            else
            {
                Debug.LogError($"Префаб {_upgradeItemPrefab.name} не содержит компонент UpgradeItemUI!");
            }
        }

        Debug.Log($"Создано {_items.Count} элементов улучшений");
    }

    public void RefreshAllItems()
    {
        foreach (var item in _items)
        {
            if (item != null)
            {
                item.UpdateUI();
            }
        }
    }

    public static void RefreshUpgradeUI()
    {
        UpgradeWindow window = FindFirstObjectByType<UpgradeWindow>();
        if (window != null)
        {
            window.RefreshAllItems();
        }
    }


}