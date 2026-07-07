using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    public float moveSpeed = 6f;
    float XAxis, YAxis;
    Vector3 size;
    public Animator anim;

    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Transform handPosition;

    private GameObject handItemObject;

    void Start()
    {
        inventory = new PlayerInventory(3);
        body = GetComponent<Rigidbody2D>();
        size = gameObject.transform.localScale;
        if (inventory == null)
            inventory = new PlayerInventory(1);
        UpdateHand();
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
        }
    }

    private void UpdateHand()
    {
        if (handItemObject != null)
        {
            Destroy(handItemObject);
            handItemObject = null;
        }

        if (!inventory.IsEmpty)
        {
            ItemData topItem = inventory.Peek();
            if (topItem != null)
            {
                handItemObject = new GameObject("HandItem");
                handItemObject.transform.SetParent(handPosition);
                handItemObject.transform.localPosition = Vector3.zero;
                handItemObject.transform.localScale = Vector3.one;

                SpriteRenderer sr = handItemObject.AddComponent<SpriteRenderer>();
                sr.sprite = topItem.Icon;
                sr.sortingOrder = 1;
            }
        }
        else
        {
            Debug.Log("Рука пуста");
        }
    }

    public void RefreshHand()
    {
        UpdateHand();
    }

    public PlayerInventory GetInventory()
    {
        return inventory;
    }
}