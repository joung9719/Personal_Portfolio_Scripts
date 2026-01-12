using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int hp = 1;
    public SpriteRenderer sr;
    public Color originalColor;

    void Awake()
    {
        sr=GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        originalColor=sr.color;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Ball"))
        {
            TakeDamage();
        }
    }
    void TakeDamage()
    {
        hp--;
         Debug.Log("블록 HP: " + hp);
        if (hp <= 0)
        {
             Debug.Log("블록 파괴됨! CheckBlocks 호출!");
             FindObjectOfType<BlockOutGamrManger>().AddScore(100);
            Destroy(gameObject);
            FindObjectOfType<BlockOutGamrManger>().CheckBlocks();
        }
        else
        {
            sr.color =originalColor*0.6f;
        }
    }
}
