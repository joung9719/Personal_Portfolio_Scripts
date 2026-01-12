using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;



public class Enemy_Ctrl : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Vector2 moveDirection;
    private Rigidbody2D rd;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        if (rd == null)
            rd = gameObject.AddComponent<Rigidbody2D>();

        rd.gravityScale = 0;
        rd.freezeRotation = true;
        rd.velocity = Vector2.zero;

        SetRandomDirection();
    }

    void FixedUpdate()
    {
        rd.velocity = moveDirection * moveSpeed;
    }

    void SetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ).normalized;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 normal = col.contacts[0].normal;

        // 벽 또는 잘린 영역이면 반사
        if (col.collider.CompareTag("Wall"))
        {
            moveDirection = Vector2.Reflect(moveDirection, normal);
        }

      
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
          if(collision.CompareTag("CutArea"))
        {
            Vector2 normal=((Vector2)transform.position-collision.ClosestPoint(transform.position)).normalized;
            moveDirection=Vector2.Reflect(moveDirection,normal);
        }
    }
}

