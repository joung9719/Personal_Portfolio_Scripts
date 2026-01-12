using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtr : MonoBehaviour
{
    public float speed=10f;
    float minY;
    float maxY;

    // Start is called before the first frame update
    void Start()
    {
      GameObject[] walls=GameObject.FindGameObjectsWithTag("Wall");

      float bottom=float.MaxValue;
      float top=float.MinValue;

      foreach(var w in walls)
        {
            if(w.transform.position.y<bottom)bottom=w.transform.position.y;
            if(w.transform.position.y>top)top=w.transform.position.y;
        }
        minY=bottom+0.5f;
        maxY=top-0.5f;    
    }

    // Update is called once per frame
    void Update()
    {
        float v=Input.GetAxisRaw("Vertical");
        Vector3 pos=transform.position;
        pos.y+=v*speed*Time.deltaTime;

        pos.y=Mathf.Clamp(pos.y,minY,maxY);
        transform.position=pos;
    }
}
