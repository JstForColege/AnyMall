using System.Collections;
using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour
{
    [Header("Инвентарь")]
    [SerializeField] private int baseInventorySize = 3;
    [SerializeField] private Transform handPosition;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    private PlayerInventory inventory;
    private GameObject handItemObject;
    private PlayerAnimation playerAnimation;

    public PlayerInventory Inventory => inventory;

    private void Start()
    {
        inventory = new PlayerInventory(baseInventorySize);

        if (handPosition == null)
            Debug.LogError("HandPosition not assigned!");

        if (playerSpriteRenderer == null)
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.Subscribe(UpgradeType.PLAYER_INVENTORY, OnInventoryUpgraded);
            int currentLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
            OnInventoryUpgraded(currentLevel);
        }

        UpdateHand();
    }

    private void OnInventoryUpgraded(int newLevel)
    {
        int newSize = baseInventorySize + newLevel;
        inventory.SetMaxSize(newSize);
        Debug.Log($"Inventory size increased to {newSize}");
    }

    public bool TryAddItem(ItemData item)
    {
        if (inventory.IsFull) return false;
        bool added = inventory.Push(item);
        if (added) UpdateHand();
        return added;
    }

    public bool TryRemoveItem(ItemData item)
    {
        if (inventory.IsEmpty) return false;
        ItemData top = inventory.Peek();
        if (top.Type != item.Type) return false;
        inventory.Pop();
        UpdateHand();
        return true;
    }

    public ItemData GetFirstItem()
    {
        return inventory.Peek();
    }

    public void RemoveTopItem()
    {
        if (!inventory.IsEmpty)
        {
            inventory.Pop();
            UpdateHand();
        }
    }

    public void RefreshHand()
    {
        UpdateHand();
    }

    private void UpdateHand()
    {
        if (handItemObject != null)
        {
            Destroy(handItemObject);
            handItemObject = null;
        }

        if (inventory.IsEmpty) return;

        ItemData topItem = inventory.Peek();
        if (topItem == null || topItem.Icon == null) return;

        handItemObject = new GameObject("HandItem");
        handItemObject.transform.SetParent(handPosition);
        handItemObject.transform.localPosition = Vector3.zero;
        handItemObject.transform.localScale = Vector3.one;

        SpriteRenderer sr = handItemObject.AddComponent<SpriteRenderer>();
        sr.sprite = topItem.Icon;
        sr.sortingOrder = 1;

        if (playerSpriteRenderer != null)
            sr.flipX = playerSpriteRenderer.flipX;
    }

    private void OnDestroy()
    {
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.Instance.Unsubscribe(UpgradeType.PLAYER_INVENTORY, OnInventoryUpgraded);
    }
}