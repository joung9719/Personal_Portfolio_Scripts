using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed=5f;
    public LayerMask WallLayer;
    public CoverMaskInit coverMask;
    public Transform startPoint;
    Vector2 input;

    void Start()
    {
        transform.position=startPoint.position;
    }
    // Update is called once per frame
    void Update()
    {
      input=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
      if(Mathf.Abs(input.x)>0.1f)
      input.y=0;
      else if(Mathf.Abs(input.y)>0.1f)
      input.x=0;
      if(input.sqrMagnitude>0)
      Move();
    }
    void Move()
    {
        Vector3 dir=new Vector3(input.x,input.y).normalized;
        Vector3 nextPos=transform.position+dir*moveSpeed*Time.deltaTime;

        if(Physics2D.OverlapPoint(nextPos,WallLayer))
        {
            return;
        }

        transform.position=nextPos;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Debug.Log("야야?");
            QixGameManager.instance.PlayerHit();
        }
    }
}
