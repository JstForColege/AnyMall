using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour
{
    [Header("Инвентарь")]
    [SerializeField] private int baseInventorySize = 3;
    [SerializeField] private Transform handPosition;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private PlayerController playerController;

    [Header("Стопка предметов")]
    [SerializeField] private float stackOffsetY = 2f;    
    [SerializeField] private float dragMultiplier = 0.1f;
    [SerializeField] private float maxDrag = 1.0f;       

    private PlayerInventory inventory;
    private List<GameObject> handItems = new List<GameObject>();

    public PlayerInventory Inventory => inventory;

    private void Start()
    {
        inventory = new PlayerInventory(baseInventorySize);

        if (handPosition == null)
            Debug.LogError("HandPosition not assigned!");
        if (playerSpriteRenderer == null)
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.Subscribe(UpgradeType.PLAYER_INVENTORY, OnInventoryUpgraded);
            int currentLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
            OnInventoryUpgraded(currentLevel);
        }
    }

    private void OnInventoryUpgraded(int newLevel)
    {
        int newSize = baseInventorySize + newLevel;
        inventory.SetMaxSize(newSize);
        Debug.Log($"Inventory size increased to {newSize}");
    }

    // --- Добавление/удаление предметов ---
    public bool TryAddItem(ItemData item)
    {
        if (inventory.IsFull) return false;
        bool added = inventory.Push(item);
        if (added)
        {
            AddHandItem(item);
        }
        return added;
    }

    public bool TryRemoveItem(ItemData item)
    {
        if (inventory.IsEmpty) return false;
        ItemData top = inventory.Peek();
        if (top.Type != item.Type) return false;
        inventory.Pop();
        RemoveTopHandItem();
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
            RemoveTopHandItem();
        }
    }

    public void RefreshHand()
    {
        ClearHandItems();
        foreach (ItemData item in inventory.GetAllItems())
        {
            AddHandItem(item);
        }
    }

    private void AddHandItem(ItemData item)
    {
        if (item == null || item.Icon == null || handPosition == null)
            return;

        GameObject newItem = new GameObject("HandItem_" + handItems.Count);
        newItem.transform.SetParent(handPosition);
        newItem.transform.localScale = Vector3.one;

        SpriteRenderer sr = newItem.AddComponent<SpriteRenderer>();
        sr.sprite = item.Icon;
        sr.sortingLayerName = playerSpriteRenderer != null ? playerSpriteRenderer.sortingLayerName : "Default";
        sr.sortingOrder = playerSpriteRenderer != null ? playerSpriteRenderer.sortingOrder + 1 : 1;
        sr.flipX = playerSpriteRenderer != null && playerSpriteRenderer.flipX;

        handItems.Add(newItem);
        UpdateHandPositions();
    }

    private void RemoveTopHandItem()
    {
        if (handItems.Count == 0) return;
        int lastIndex = handItems.Count - 1;
        GameObject obj = handItems[lastIndex];
        if (obj != null) Destroy(obj);
        handItems.RemoveAt(lastIndex);
        UpdateHandPositions();
    }

    private void ClearHandItems()
    {
        foreach (GameObject obj in handItems)
            if (obj != null) Destroy(obj);
        handItems.Clear();
    }

    private void UpdateHandPositions()
    {
        if (handItems.Count == 0) return;

        Vector2 moveInput = playerController != null ? playerController.GetMoveInput() : Vector2.zero;
        float velocityX = moveInput.x;
        bool isMoving = Mathf.Abs(velocityX) > 0.1f;

        for (int i = 0; i < handItems.Count; i++)
        {
            GameObject obj = handItems[i];
            if (obj == null) continue;

            float offsetY = i * stackOffsetY;
            float dragX = 0f;
            if (isMoving)
            {
                float drag = -velocityX * dragMultiplier * (i + 1);
                dragX = Mathf.Clamp(drag, -maxDrag, maxDrag);
            }

            obj.transform.localPosition = new Vector3(dragX, offsetY, 0f);
        }
    }

    public void UpdateHandFlip()
    {
        if (playerSpriteRenderer == null) return;
        bool flip = playerSpriteRenderer.flipX;
        foreach (GameObject obj in handItems)
        {
            if (obj == null) continue;
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = flip;
        }
    }

    private void Update()
    {
        UpdateHandPositions();
    }

    private void OnDestroy()
    {
        ClearHandItems();
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.Instance.Unsubscribe(UpgradeType.PLAYER_INVENTORY, OnInventoryUpgraded);
    }
}