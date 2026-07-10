using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        Vector2 moveInput = playerController.GetMoveInput();
        bool isMoving = moveInput.magnitude > 0.1f;

        if (animator != null)
            animator.SetBool("isMoving", isMoving);

        if (spriteRenderer != null)
        {
            if (moveInput.x > 0)
                spriteRenderer.flipX = false;
            else if (moveInput.x < 0)
                spriteRenderer.flipX = true;
        }
    }
}