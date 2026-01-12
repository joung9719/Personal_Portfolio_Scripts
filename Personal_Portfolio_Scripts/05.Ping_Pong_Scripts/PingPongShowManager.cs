using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PingPongShowManager : MonoBehaviour
{
    public float wait = 1.5f;

    IEnumerator Start()
    {
        int stage = PingPongGameDate.CurrentStage;

      
        RemoveCover("Cover01");    
        if (stage >= 2) RemoveCover("Cover02"); 
        if (stage >= 3) RemoveCover("Cover03");

        yield return new WaitForSeconds(wait);


        var ch = PingPongGameDate.SelectedCharacter;
        var stageList = PingPongStageDB.StageScenes[ch];

       
        int nextIndex = stage - 1;

      
        if (stage >= stageList.Length)
        {
            PingPongGameDate.clearCharaters[(int) ch]=true;
            string clearScene = PingPongStageDB.ClearScenes[ch];
            SceneManager.LoadScene(clearScene);
            yield break;
        }

      
        PingPongGameDate.CurrentStage++;
        SceneManager.LoadScene(stageList[nextIndex]);
    }

    void RemoveCover(string tag)
    {
        var obj = GameObject.FindGameObjectWithTag(tag);
        if (obj != null) Destroy(obj);
    }
}
