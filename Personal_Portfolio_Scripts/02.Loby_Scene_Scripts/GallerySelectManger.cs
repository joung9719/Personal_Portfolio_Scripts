using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GallerySelectManger : MonoBehaviour
{
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("01.Start");
        }  
    }

    public void GoQixGallery()
    {
        SceneManager.LoadScene("QixGallery");
    }

    public void GoBlockOutGallery()
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
