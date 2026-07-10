using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class WalletSaveData
{
    public int money;
    public List<string> purchasedZones;
}

public class PlayerWallet : MonoBehaviour, IWalletAccess
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private int money = 0;
    private List<string> purchasedZones = new List<string>();
    private const string SAVE_KEY = "wallet";

    private void Awake()
    {
        purchasedZones = new List<string>();
    }

    private void Start()
    {
        LoadWallet();
        UpdateUI();

        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.Initialize(this);
            Debug.Log("PlayerWallet: UpgradeSystem initialized.");

            UpgradeWindow window = FindFirstObjectByType<UpgradeWindow>();
            if (window != null)
            {
                window.RefreshAllItems();
            }
        }
    }

    public int GetMoney()
    {
        return money;
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            SaveWallet();
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        StartCoroutine(AddMoneyCoroutine(amount));
    }

    private IEnumerator AddMoneyCoroutine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            money++;
            UpdateUI();
            yield return new WaitForSeconds(0.05f);
        }
        SaveWallet();
    }

    public bool IsZonePurchased(string zoneId)
    {
        return purchasedZones.Contains(zoneId);
    }

    public void PurchaseZone(string zoneId, int price)
    {
        if (IsZonePurchased(zoneId) || !SpendMoney(price))
            return;

        purchasedZones.Add(zoneId);
        SaveWallet();
    }

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }

    public void SaveWallet()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogWarning("SaveSystem not ready, wallet not saved");
            return;
        }

        WalletSaveData data = new WalletSaveData();
        data.money = money;
        data.purchasedZones = purchasedZones;
        SaveSystem.Instance.SaveObject(SAVE_KEY, data);
    }

    private void LoadWallet()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogWarning("SaveSystem not ready, wallet not loaded");
            return;
        }

        object raw = SaveSystem.Instance.LoadObject(SAVE_KEY);
        if (raw != null)
        {
            WalletSaveData data = raw as WalletSaveData;
            if (data != null)
            {
                money = data.money;
                purchasedZones = data.purchasedZones ?? new List<string>();
                UpdateUI();
                Debug.Log($"Wallet loaded {money} money, {purchasedZones.Count} zones");
            }
        }
        else
        {
            Debug.Log("No wallet save found, starting fresh");
        }
    }

    private void OnApplicationQuit()
    {
        SaveWallet();
    }
}