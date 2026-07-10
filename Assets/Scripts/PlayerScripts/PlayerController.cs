using Assets.Scripts.Monobehavior_Script;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Движение")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Инвентарь и руки")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private int baseInventorySize = 3;
    [SerializeField] private Transform handPosition;

    private Vector2 moveInput;
    private GameObject handItemObject;
    private bool isMoving = false;

    private void Start()
    {
        if (body == null)
            body = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        inventory = new PlayerInventory(baseInventorySize);

        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.Subscribe(UpgradeType.PLAYER_INVENTORY, OnInventoryUpgraded);

            int currentLevel = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
            OnInventoryUpgraded(currentLevel);
        }
        else
        {
            Debug.LogWarning("UpgradeSystem не найден, размер инвентаря не будет улучшаться.");
        }

        UpdateHand();
        Debug.Log($"MaxSize = {inventory.MaxSize}");
    }

    private void Update()
    {
        HandleInput();
        SpriteFlip();
        Animation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region Движение

    private void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isMoving = moveInput.magnitude > 0.1f;
    }

    private void SpriteFlip()
    {
        if (spriteRenderer == null) return;
        if (moveInput.x > 0)
            spriteRenderer.flipX = false;
        else if (moveInput.x < 0)
            spriteRenderer.flipX = true;
    }

    private void Move()
    {
        if (body != null)
            body.linearVelocity = moveInput * moveSpeed;
    }

    #endregion

    #region Анимация

    private void Animation()
    {
        if (animator != null)
            animator.SetBool("isMoving", isMoving);
    }

    #endregion

    #region Взаимодействие

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ResourceNode node))
        {
            if (inventory.IsFull) return;
            ItemData item = node.Harvest();
            if (item != null)
            {
                if (!inventory.Push(item)) return;
                UpdateHand();
            }
            return;
        }
        if (other.TryGetComponent(out Storage shelf))
        {
            if (inventory.IsEmpty) return;
            Debug.Log("Вижу полку");
            ItemData item = inventory.Peek();
            if (shelf.AddItem(item))
            {
                inventory.Pop();
                UpdateHand();
                Debug.Log("выложил");
            }
            return;
        }
        if (other.TryGetComponent(out ProducerInput input))
        {
            if (inventory.IsEmpty) return;
            ItemData item = inventory.Peek();
            Debug.Log("Вижу что-то аааааааааааааааааа");
            if (input.producer.AddItem(item))
            {
                Debug.Log("Положил что-то аааааааааааааааааа");
                inventory.Pop();
                UpdateHand();
            }
            return;
        }
        if (other.TryGetComponent(out ProducerOutput output))
        {
            if (inventory.IsFull) return;
            ItemData item = output.producer.TakeOutgridient();
            if (item != null)
            {
                inventory.Push(item);
                UpdateHand();
            }
            return;
        }

        HooliganAI hooligan = other.GetComponent<HooliganAI>();
        if (hooligan != null)
        {
            bool chased = hooligan.TryChaseAway();
            if (chased)
            {
                Debug.Log("Игрок: Прогнан");
            }
            else
            {
                Debug.Log("Игрок: Хулиган ещё не у стеллажа!");
            }
            return;
        }

        // надо: взаимодействие с другими объектами через IInteractable
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out CashRegister cashRegister))
        {
            bool served = cashRegister.TryServeNextCustomer();
            if (served)
            {
                Debug.Log("Player: Served customer at cash!");
            }
            return;
        }

    }
    #endregion

        #region Инвентарь и руки

    public PlayerInventory GetInventory()
    {
        return inventory;
    }

    public void RefreshHand()
    {
        UpdateHand();
    }
    private void OnInventoryUpgraded(int newLevel)
    {
        int newSize = baseInventorySize + newLevel;
        inventory.SetMaxSize(newSize);
        Debug.Log($"Размер инвентаря увеличен до {newSize}");

    }

    private void UpdateHand()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is null in UpdateHand!");
            return;
        }

        if (handItemObject != null)
        {
            Destroy(handItemObject);
            handItemObject = null;
        }

        if (inventory.IsEmpty)
        {
            Debug.Log("Рука пуста");
            return;
        }

        ItemData topItem = inventory.Peek();
        if (topItem == null) return;

        if (handPosition == null)
        {
            Debug.LogError("HandPosition is null in UpdateHand!");
            return;
        }

        handItemObject = new GameObject("HandItem");
        handItemObject.transform.SetParent(handPosition);
        handItemObject.transform.localPosition = Vector3.zero;
        handItemObject.transform.localScale = Vector3.one;

        SpriteRenderer sr = handItemObject.AddComponent<SpriteRenderer>();
        sr.sprite = topItem.Icon;
        sr.sortingOrder = 1;

        if (spriteRenderer != null)
            sr.flipX = spriteRenderer.flipX;
    }

    #endregion
}