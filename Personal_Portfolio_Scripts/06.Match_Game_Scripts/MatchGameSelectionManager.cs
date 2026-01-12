using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchGameSelectionManager : MonoBehaviour
{

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("02.Lobby");
        }
    }
    void StartGame(MatchGameCharacter ch)
    {
        MatchGameGameData.SelectedCharacter=ch;
        MatchGameGameData.CurrentStage=0;

       
        SceneManager.LoadScene("02.MatchGame");

        MatchGameGameData.life=3;
        MatchGameGameData.score=0;
        MatchGameGameData.round=0;
        MatchGameGameData.remainTime=900f;
    }

    public void SelectIchinoseAsuna()=>StartGame(MatchGameCharacter.IchinoseAsuna);
    public void SelectKosakaWakamo()=>StartGame(MatchGameCharacter.KosakaWakamo);
    public void SelectMisonoMika()=>StartGame(MatchGameCharacter.MisonoMika);
    public void SelectAsagiMutsuki()=>StartGame(MatchGameCharacter.AsagiMutsuki);

    
}
