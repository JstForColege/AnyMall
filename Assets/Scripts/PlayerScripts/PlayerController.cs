using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManipulator : MonoBehaviour
{
    Rigidbody2D body;
    public float speed;
    float XAxis, YAxis;
    Vector3 size;


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
        body.linearVelocity = new Vector2(XAxis * speed, YAxis * speed);
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