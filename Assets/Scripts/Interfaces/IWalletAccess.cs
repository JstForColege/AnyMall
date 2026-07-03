using UnityEngine;

public interface IWalletAccess
{
    int GetMoney();
    bool SpendMoney(int amount);
    void AddMoney(int amount);
    bool IsZonePurchased(string zoneId);
    void PurchaseZone(string zoneId, int price);
}
