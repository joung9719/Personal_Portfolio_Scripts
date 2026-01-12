using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using Unity.Mathematics;

using UnityEngine;
using UnityEngine.UIElements;
[RequireComponent(typeof(PolygonCollider2D))]
public class CoverMaskInit : MonoBehaviour
{
    public SpriteRenderer background;
    public Texture2D maskTexture;
    public Color eraseColor = new Color(0, 0, 0, 0);

    public int brushRadius=10;
    public int brushFeather=4;

    [Range(0f,0.2f)]
    public float removerAlphaThreshold=0.01f;
    private int w,h;
    private int removedCount;
    private int totalCount;
    private  byte therSholdByte;
    private Color32[]buffer;

    void Start()
    {
        transform.position = background.transform.position;
        Create2DMask();
        CreateMaskTexture();
    }

    void Create2DMask()
    {
        Bounds b = background.bounds;

        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(b.min.x, b.min.y);
        points[1] = new Vector2(b.min.x, b.max.y);
        points[2] = new Vector2(b.max.x, b.max.y);
        points[3] = new Vector2(b.max.x, b.min.y);

        col.pathCount = 1;
        col.SetPath(0, points);
    }
    public Vector2 WorldToUV(Vector2 world)
    {
        Bounds b = background.bounds;

        float u=Mathf.InverseLerp(b.min.x,b.max.x,world.x);
        float v=Mathf.InverseLerp(b.min.y,b.max.y,world.y);

        int px=Mathf.Clamp(Mathf.FloorToInt(u*maskTexture.width),0,maskTexture.width-1);
        int py=Mathf.Clamp(Mathf.FloorToInt(v*maskTexture.height),0,maskTexture.height-1);

        return new Vector2(px,py);
    }
    void CreateMaskTexture()
    {
       w=background.sprite.texture.width;
       h=background.sprite.texture.height;

       maskTexture=new Texture2D(w,h,TextureFormat.RGBA32,false);
       maskTexture.wrapMode=TextureWrapMode.Clamp;
       maskTexture.filterMode=FilterMode.Bilinear;

        buffer = new Color32[w * h];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = new Color32(0, 0, 0, 255);
        removedCount=0;
        totalCount=buffer.Length;
        therSholdByte=(byte)Mathf.RoundToInt(removerAlphaThreshold*255f);
        maskTexture.SetPixels32(buffer);
        maskTexture.Apply(false);

        SpriteRenderer sr=GetComponent<SpriteRenderer>();
        float ppu=background.sprite.pixelsPerUnit;
        sr.sprite=Sprite.Create(maskTexture,new Rect(0,0,w,h),new Vector2(0.5f,0.5f),ppu);

    }

    public void EraseCirclePixel(int cx,int cy)
    {
        int r=brushRadius;
        int inner=Mathf.Max(0,r-brushFeather);
        int r2=r*r;
        int inner2=inner*inner;

        for (int y=-r;y<=r;y++)
        {
            int yy=cy+y;
            if((uint)yy>=(uint)h)
            continue;
            for(int x=-r;x<=r;x++)
            {
                int xx=cx+x;
                if((uint)xx>=(uint)w)
                continue;

                int d2=x*x+y*y;;
                if(d2>r2)
                continue;

                float a;
                if(d2<=inner2)
                a=0f;
                else
                {
                    float d=Mathf.Sqrt(d2);
                    float t=Mathf.InverseLerp(r,inner,d);
                    a=1f-Mathf.SmoothStep(0f,1f,t);
                }

                int idx=yy*w+xx;
                byte oldA=buffer[idx].a;
                byte newA=(byte)Mathf.Clamp(Mathf.RoundToInt(a*255f),0,255);

                if(newA>=oldA)
                continue;

                if(oldA>therSholdByte&&newA<=therSholdByte)
                removedCount++;

                var c=buffer[idx];
                c.a=newA;
                buffer[idx]=c;

            }
        }
    }

    public void EraseAt(Vector2 world)
    {
        Vector2 uv=WorldToUV(world);
        EraseCirclePixel((int)uv.x,(int)uv.y);
    }

    public void ApplyMask()
    {
        maskTexture.SetPixels32(buffer);
        maskTexture.Apply(false,false);
    }

    public float GetRemovedPercent()
    {
        return(float)removedCount/totalCount*100f;
    }

    public void ErasePixel(int x,int y)
    {
        if((uint)x>=(uint)w||(uint)y>=(uint)h)
        return;

        int idx=y*w+x;
        byte oldA=buffer[idx].a;
        if(oldA<=therSholdByte)
        return;

        var c=buffer[idx];
        c.a=0;
        buffer[idx]=c;
        removedCount++;
    }

}
