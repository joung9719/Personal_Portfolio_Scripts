using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;


public class AreaManager : MonoBehaviour
{
    public CoverMaskInit mask;
    public int rowsPerFrame=32;
    

    List<Vector3>SortVertices(List<Vector3>list)
    {
        Vector3 center=Vector3.zero;
        foreach(var p in list)
        center+=p;
        center/=list.Count;

        list.Sort((a,b)=>
        {
            float angA=Mathf.Atan2(a.y-center.y,a.x-center.x);
            float angB=Mathf.Atan2(b.y-center.y,b.x-center.x);
            return angA.CompareTo(angB);
        });

        return list;
    }
    public void CutArea(List<Vector3> polygonWorld)
    {
        StopAllCoroutines();
        StartCoroutine(CutAreaRoutine(polygonWorld));
    }

    IEnumerator CutAreaRoutine(List<Vector3>polygonWorld)
    {
        if(polygonWorld==null||polygonWorld.Count<3||mask==null)
        yield break;

        polygonWorld=SortVertices(polygonWorld);
        List<Vector2Int> poly=new List<Vector2Int>(polygonWorld.Count);
        int minX=int.MaxValue,minY=int.MaxValue,maxX=int.MinValue,maxY=int.MinValue;

        foreach(var wp in polygonWorld)
        {
            Vector2 uv=mask.WorldToUV(wp);
            int x=(int)uv.x;
            int y=(int)uv.y;
            poly.Add(new Vector2Int(x,y));
            if(x<minX)minX=x;
            if(y<minY)minY=y;
            if(x>maxX)maxX=x;
            if(y>maxY)maxY=y;
        }

        int rowCount=0;

        for(int y=minY;y<=maxY;y++)
        {
            for(int x=minX;x<=maxX;x++)
            {
                if(IsInsidePixels(poly,x+0.5f,y+0.5f))
                mask.ErasePixel(x,y);
            }

            rowCount++;
            if(rowCount>=rowsPerFrame)
            {
                rowCount=0;
                yield return null;
            }
        }

        mask.ApplyMask();
        QixGameManager.instance.UpdatePercent();
    }

    bool IsInsidePixels(List<Vector2Int>poly,float px,float py)
    {
        bool inside=false;
        for(int i=0,j=poly.Count-1;i < poly.Count;j=i++)
        {
            float xi=poly[i].x,yi=poly[i].y;
            float xj=poly[j].x,yj=poly[j].y;

            bool intersect = ((yi > py) != (yj > py)) &&
                 (px < (xj - xi) * (py - yi) / (yj - yi + 0.00001f) + xi);


            if(intersect)
            inside=!inside;
        }
        return inside;
    }
    // bool IsInside(List<Vector3>poly,Vector2 p)
    // {
    //     bool inside=false;

    //     for(int i=0,j=poly.Count-1;i<poly.Count;j=i++)
    //     {
    //         if(((poly[i].y>p.y)!=(poly[j].y>p.y))&&
    //         (p.x<(poly[j].x-poly[i].x)*(p.y-poly[i].y)/(poly[j].y-poly[i].y)+poly[i].x))
    //         {
    //             inside=!inside;
    //         }
    //     }
    //     return inside;
    // }
    // Bounds GetBounds(List<Vector3>poly)
    // {
    //     Bounds b=new Bounds(poly[0],Vector3.zero);
    //     for(int i=1;i<poly.Count;i++)
    //     {
    //         b.Encapsulate(poly[i]);
    //     }
    //     return b;
    // }

}
