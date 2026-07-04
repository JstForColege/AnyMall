using UnityEngine;

public interface IUIUpdate
{
    public void UpdateMoney(int newMoney);
    public void UpdateUpgradeUI(UpgradeType type, int newLevel);
    public void UpdateBonusUI(BonusType type, float remainingTime, bool isActive);
    public void ShowHint(string message);
}