using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
   public bool isLeftGoal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ball"))
        {
            Destroy(collision.gameObject);
            if(isLeftGoal)
            {
                PingPongGameManager.Instance.AddScore(false);
            }
            else
            {
                PingPongGameManager.Instance.AddScore(true);
            }
        }
    }
}
