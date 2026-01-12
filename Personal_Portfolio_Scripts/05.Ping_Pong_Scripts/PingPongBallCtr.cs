using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongBallCtr : MonoBehaviour
{
    public static PingPongBallCtr Instance;
    public float ballSpeed=5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        Instance=this;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
    }

    public void ResetBall(int round,Vector3 spawnPos,int direction)
    {
        if(rb==null)
        rb=GetComponent<Rigidbody2D>();
        transform.position=spawnPos;    
        
        
        float speed=ballSpeed;
        if(round==2)speed*=1.5f;
        else if(round==3)speed*=2f;

        float y= Random.Range(-1f,1f);
        if(Mathf.Abs(y)<0.3f)y=0.3f*Mathf.Sign(y);
        
        Vector2 dir=new Vector2(direction,y);
        rb.velocity=dir.normalized*speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Paddle"))
        {
            float paddleY=collision.collider.transform.position.y;
            float ballY=transform.position.y;

            float paddleHeigth=collision.collider.bounds.size.y/2f;

            float normalizedY=(ballY-paddleY)/paddleHeigth;

            Vector2 newDir=new Vector2(Mathf.Sign(rb.velocity.x),normalizedY);

            float newSpeed=rb.velocity.magnitude+0.5f;
            rb.velocity=newDir.normalized*newSpeed;


        }
    }
}
