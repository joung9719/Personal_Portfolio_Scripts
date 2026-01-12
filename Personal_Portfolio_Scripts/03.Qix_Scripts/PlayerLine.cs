
using System.Collections.Generic;
using UnityEngine;

public class PlayerLine : MonoBehaviour
{
    public LineRenderer line;
    public bool drawing = false;
    public float minDistance = 0.05f;
    public LayerMask wallLayer;
    public AreaManager area;
    public Transform player;


    List<Vector3> points = new List<Vector3>();
    // Update is called once per frame
    void Update()
    {
       
        transform.position=player.position;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartLine();
        }


        if (Input.GetKeyUp(KeyCode.Space))
        {
            EndLine();
        }
        
    }
    void LateUpdate()
    {
        if (drawing)
        {
            AddPoint();
            if (HitWall())
                EndLine();
        }
    }

    bool HitWall()
    {
        return Physics2D.OverlapCircle(player.position, 0.05f, wallLayer);
    }

    void StartLine()
    {
        drawing = true;
        points.Clear();

        line.positionCount = 0;
        line.useWorldSpace=true;

        var col = GetComponent<EdgeCollider2D>();
        if (col == null)
            col = gameObject.AddComponent<EdgeCollider2D>();
        col.enabled = true;
        col.isTrigger = true;

        var rigid = GetComponent<Rigidbody2D>();
        if (rigid == null)
            rigid = gameObject.AddComponent<Rigidbody2D>();
        rigid.isKinematic = true;
        rigid.simulated = true;
        rigid.gravityScale = 0;
        rigid.freezeRotation = true;

        AddPoint();
    }

    void EndLine()
    {
        if (!drawing)
            return;
        drawing = false;
        Debug.Log("LINE CLOSED!");
        area.CutArea(points);
        line.positionCount = 0;

           var col=GetComponent<EdgeCollider2D>();
           if(col!=null)
           col.enabled=false;
    }

    void AddPoint()
    {
        Vector3 pos = player.position;
        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], pos) > minDistance)
        {
            points.Add(pos);
            line.positionCount = points.Count;
            line.SetPosition(points.Count - 1, pos);
            UpdateCollider();

        }
    }

    void UpdateCollider()
    {
        var col = GetComponent<EdgeCollider2D>();
        if (col == null)
            col = gameObject.AddComponent<EdgeCollider2D>();
        List<Vector2> pts = new List<Vector2>();
        foreach (var p in points)
        {
            Vector2 local=transform.InverseTransformPoint(p);
            pts.Add(local);
        }
        col.SetPoints(pts);
        col.edgeRadius = 0.01f;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Debug.Log("라인에 닿아서 죽음");
            QixGameManager.instance.PlayerHit();
        }
    }
}
