using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public void GoLobby()
    {
        int next=SceneManager.GetActiveScene().buildIndex+1;
        SceneManager.LoadScene(next);    
    }
    
    public void GameExit()
    {
        Application.Quit();
    }

    public void GoGallery()
    {
        SceneManager.LoadScene("03.Gallery");
    }
}
