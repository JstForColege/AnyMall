using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    public float moveSpeed = 6f;
    float XAxis, YAxis;
    Vector3 size;
    public Animator anim;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        size = gameObject.transform.localScale;
    }

    void Update()
    {
        Move();
    }
    void Move()
    {
        XAxis = Input.GetAxis("Horizontal");
        YAxis = Input.GetAxis("Vertical");
        bool isMoving = body.linearVelocity.magnitude > 0.1f;
        anim.SetBool("isMoving", isMoving);
        body.linearVelocity = new Vector2(XAxis * moveSpeed, YAxis * moveSpeed);
        if (XAxis > 0)
        {
            gameObject.transform.localScale = size;
        }
        if (XAxis < 0)
        {
            gameObject.transform.localScale = new Vector3(-size.x, size.y, size.z);
        }
    }
}