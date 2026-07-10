using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Rigidbody2D body;

    private Vector2 moveInput;

    private void Start()
    {
        if (body == null)
            body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        if (body != null)
            body.linearVelocity = moveInput * moveSpeed;
    }

    public Vector2 GetMoveInput() => moveInput;
}