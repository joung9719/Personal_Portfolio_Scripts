using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QixSelectionMager : MonoBehaviour
{
    void Update()
    {
         if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("02.Lobby");
        }
    }
    public void GoAlice()
    {
       QixGameData.currentCharater=QixCharacterType.Sora;
       QixGameData.CurrentStage=0;
       SceneManager.LoadScene("02.QixGame");
    }

    public void GoD()
    {
        QixGameData.currentCharater=QixCharacterType.D;
       QixGameData.CurrentStage=0;
       SceneManager.LoadScene("02.QixGame");
    }

    public void GoNayuta()
    {
        QixGameData.currentCharater=QixCharacterType.Nayuta;
       QixGameData.CurrentStage=0;
       SceneManager.LoadScene("02.QixGame");
    }

    public void GoLiberalio()
    {
        QixGameData.currentCharater=QixCharacterType.Liberialo;
       QixGameData.CurrentStage=0;
       SceneManager.LoadScene("02.QixGame");
    }

}
