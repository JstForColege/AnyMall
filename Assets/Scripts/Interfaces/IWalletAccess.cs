using UnityEngine;

public interface IWalletAccess
{
    public int GetMoney();
    public bool SpendMoney(int amount);
    public void AddMoney(int amount);
    public bool IsZonePurchased(string zoneId);
    public void PurchaseZone(string zoneId, int price);
}
