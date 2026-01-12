using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Windows.Speech;

public class BallCltr : MonoBehaviour
{
    public float ballSpeed=8f;
     Rigidbody2D rb;
    public bool isShot=false;
    
     public Transform paddle;
     public Vector2 launchDir=Vector2.up;
    public float minAxis=0.12f;
    public float maxBouncAngle=60f;

    private Vector2 lastVelocity;
    private const float pushOut=0.01f;
    // Start is called before the first frame update
    void Start()
    {
       rb=GetComponent<Rigidbody2D>();
       rb.velocity=Vector2.zero;

       rb.gravityScale=0f;
       rb.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
       rb.interpolation=RigidbodyInterpolation2D.Interpolate;
       
    }
    void Update()
    {
       if(!isShot)
        {
            Vector3 pos=transform.position;
            pos.x=paddle.position.x;
            transform.position=pos;
            if(Input.GetKeyDown(KeyCode.Space))
            {
                isShot=true;
               SetVelocity(launchDir);
               lastVelocity=rb.velocity;
            }
        }
    }
    void FixedUpdate()
    {
        if(isShot&&rb!=null)
        lastVelocity=rb.velocity;
    }
    void SetVelocity(Vector2 dir)
    {
        if(rb==null)
        return;
        rb.velocity=dir.normalized*ballSpeed;
    }

    Vector2 FixTooStraight(Vector2 dir,Vector2? wallNormal=null)
    {
        if(Mathf.Abs(dir.x)<minAxis)
        {
            if(wallNormal.HasValue&&Mathf.Abs(wallNormal.Value.x)>0.5f)
            dir.x=Mathf.Sign(wallNormal.Value.x)*minAxis;
            else
            dir.x=Mathf.Sign(dir.x==0f?1f:dir.x)*minAxis;
        }
        if(Mathf.Abs(dir.y)<minAxis)
        {
            if(wallNormal.HasValue&&Mathf.Abs(wallNormal.Value.y)>0.5f)
            dir.y=Mathf.Sign(wallNormal.Value.y)*minAxis;
            else
            dir.y=Mathf.Sign(dir.y==0f?1f:dir.y)*minAxis;
        }
        return dir.normalized;
    }
    Vector2 GetPaddleBounceDir(Collider2D paddleCol)
    {
        float centerX=paddleCol.bounds.center.x;
        float halfW=paddleCol.bounds.extents.x;

        float hit=(transform.position.x-centerX)/halfW;
    hit=Mathf.Clamp(hit,-1f,1f);
    float angleRad=hit*maxBouncAngle*Mathf.Deg2Rad;

    Vector2 dir=new Vector2(Mathf.Sin(angleRad),Mathf.Cos(angleRad));
    return FixTooStraight(dir);
    }
   

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(rb==null)
        return;
        if(collision.collider.CompareTag("Paddel"))
        {
            Vector2 dir=GetPaddleBounceDir(collision.collider);
            SetVelocity(dir);
            return;
        }

        if(collision.collider.CompareTag("Wall"))
        {
            Vector2 normal=collision.contacts[0].normal;
           Vector2 incoming=(lastVelocity.sqrMagnitude>0.0001f)?lastVelocity:rb.velocity;
           Vector2 dir=Vector2.Reflect(incoming.normalized,normal);
           dir=FixTooStraight(dir,normal);
           SetVelocity(dir);
           rb.position+=normal*pushOut;
           return;
        }
        if(collision.collider.CompareTag("End"))
        {
            FindObjectOfType<BlockOutGamrManger>().LoseLife();
            Destroy(gameObject);
        }
    }

    void ResetBall()
    {
      rb.velocity=Vector2.zero;
      isShot=false;
      transform.position=new Vector2(0,-4);   
    }


}
