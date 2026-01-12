using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaddleCtrl : MonoBehaviour
{
    public float speed=10f;
    float minX;
    float maxX;
    public Transform ball;

    void Start()
    {
        GameObject[]walls=GameObject.FindGameObjectsWithTag("Wall");
        float left=walls.Min(w=>w.transform.position.x);
        float right=walls.Max(w=>w.transform.position.x);

        float offset=0.5f;

        minX=left+offset;
        maxX=right-offset;

       

    }
    // Update is called once per frame
    void Update()
    {
      float h=Input.GetAxisRaw("Horizontal");
      Vector3 pos=transform.position;
      pos.x+=h*speed*Time.deltaTime;
      pos.x=Mathf.Clamp(pos.x,minX,maxX);
      transform.position=pos;

       if(!ball.GetComponent<BallCltr>().isShot)
        {
            Vector3 ballpos=ball.position;
            ballpos.x=transform.position.x;
            ball.position=ballpos;
        }

    }
}
