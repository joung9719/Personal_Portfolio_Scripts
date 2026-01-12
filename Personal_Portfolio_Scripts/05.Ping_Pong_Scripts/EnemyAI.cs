using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform ball;

    public float enemySpeed=3f;
    private float delay;

    void Start()
    {
       FindBall();
        ChangeDifficulty(PingPongGameDate.CurrentStage);
    }
    void Update()
    {
        if(ball==null)
        {
            FindBall();
            return;
        }
        float targetY=ball.position.y;
        Vector3 pos=transform.position;
        float move=enemySpeed*Time.deltaTime;
        float diff=targetY-pos.y;

        if(Mathf.Abs(diff)>move)
        pos.y+=Mathf.Sign(diff)*move;
        else
        pos.y=targetY;
        transform.position=pos;
        
    }

    public void ChangeDifficulty(int round)
    {
        if(round==1)
        {
            enemySpeed=3f;
            delay=0.25f;
        }
        else if(round==2)
        {
            enemySpeed=4.5f;
            delay=0.12f;
        }
        else if(round==3)
        {
            enemySpeed=6f;
            delay=0.06f;
        }
    }

    void FindBall()
    {
        var b=FindObjectOfType<PingPongBallCtr>();
        if(b!=null)
        {
            ball=b.transform;
        }
        else
        {
            ball=null;

        }
    }
   
   
}
