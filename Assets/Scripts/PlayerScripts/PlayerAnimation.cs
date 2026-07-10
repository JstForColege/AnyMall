using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInventoryHandler inventoryHandler;

    private Vector2 previousMoveInput;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
        if (inventoryHandler == null)
            inventoryHandler = GetComponent<PlayerInventoryHandler>();
    }

    private void Update()
    {
        Vector2 moveInput = playerController.GetMoveInput();
        bool isMoving = moveInput.magnitude > 0.1f;

        // Анимация
        if (animator != null)
            animator.SetBool("isMoving", isMoving);

        // Поворот спрайта
        if (spriteRenderer != null)
        {
            if (moveInput.x > 0)
                spriteRenderer.flipX = false;
            else if (moveInput.x < 0)
                spriteRenderer.flipX = true;

            // Если направление изменилось, обновляем предмет в руке
            if (moveInput != previousMoveInput && inventoryHandler != null)
            {
                inventoryHandler.UpdateHandFlip();
                previousMoveInput = moveInput;
            }
        }
    }
}