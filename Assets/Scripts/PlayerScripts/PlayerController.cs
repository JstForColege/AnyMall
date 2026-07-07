using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Движение")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Инвентарь и руки")]
    [SerializeField] private PlayerInventory inventory;
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

        if (inventory == null)
            inventory = new PlayerInventory(1);

        // Надо: интегрировать с UpgradeSystem
        // int level = UpgradeSystem.Instance.GetCurrentLevel(UpgradeType.PLAYER_INVENTORY);
        // inventory = new PlayerInventory(1 + level);

        UpdateHand();
    }

    private void Update()
    {
        HandleInput();
        Move();
        Animation();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    #region Движение

    private void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isMoving = moveInput.magnitude > 0.1f;
    }

    private void Move()
    {
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void ApplyMovement()
    {
        if (body != null)
        {
            body.linearVelocity = moveInput * moveSpeed;
        }
    }

    #endregion

    #region Анимация

    private void Animation()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }

    #endregion

    #region Взаимодействие

    private void OnTriggerEnter2D(Collider2D other)
    {
        ResourceNode node = other.GetComponent<ResourceNode>();
        if (node != null)
        {
            ItemData item = node.Harvest();
            if (item != null)
            {
                bool added = inventory.Push(item);
                if (added)
                {
                    UpdateHand();
                }
            }
            return;
        }

        // надо: Взаимодействие с другими объектами через IInteractable
        // IInteractable interactable = other.GetComponent<IInteractable>();
        // if (interactable != null)
        // {
        //     interactable.OnInteract(this);
        // }
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

    private void UpdateHand()
    {
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

        handItemObject = new GameObject("HandItem");
        handItemObject.transform.SetParent(handPosition);
        handItemObject.transform.localPosition = Vector3.zero;
        handItemObject.transform.localScale = Vector3.one;

        SpriteRenderer sr = handItemObject.AddComponent<SpriteRenderer>();
        sr.sprite = topItem.Icon;
        sr.sortingOrder = 1;

        sr.flipX = spriteRenderer.flipX;
    }

    #endregion
}