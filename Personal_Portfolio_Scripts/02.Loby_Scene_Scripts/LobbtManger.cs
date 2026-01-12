using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbtManger : MonoBehaviour
{
    void Update()
    {
         if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("01.Start");
        }
    }
    public void GoQix()
    {
        SceneManager.LoadScene("01.QixSelection");
    }

    public void GoBlockOut()
    {
        SceneManager.LoadScene("01.BlockSelection");
    }

    public void GoPingPong()
    {
        SceneManager.LoadScene("01.PingPongSelection");
    }

    public void GoMatchGame()
    {
        SceneManager.LoadScene("01.MatchGameSelection");
    }


    public void GoQixGallery()
    {
        SceneManager.LoadScene("QixGallery");
    }

    public void GoBlockGallery()
    {
        SceneManager.LoadScene("BlockOutGallery");
    }

    public void GoPingPongGallery()
    {
        SceneManager.LoadScene("PingPongGallery");
    }

    public void GoMatchGameGallery()
    {
        SceneManager.LoadScene("MatchGameGallery");
    }
}
