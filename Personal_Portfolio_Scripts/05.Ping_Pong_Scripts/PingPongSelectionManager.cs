using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PingPongSelectionManager : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("02.Lobby");
        }
    }
    void StartGame(PingPongCharacter ch)
    {
        PingPongGameDate.SelectedCharacter=ch;
        PingPongGameDate.CurrentStage=1;

        string firstStage=PingPongStageDB.StageScenes[ch][0];
        SceneManager.LoadScene(firstStage);

       PingPongGameManager.life=3;
    }

    public void SelectEllenJoe()=>StartGame(PingPongCharacter.EllenJoe);
    public void SelectMiyabi()=>StartGame(PingPongCharacter.Miyabi);
    public void SelectXyuan()=>StartGame(PingPongCharacter.Xyuan);
    public void SelectJaneDoe()=>StartGame(PingPongCharacter.JanDoe);
}
