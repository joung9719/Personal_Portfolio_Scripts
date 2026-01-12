using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockOutSelectionManger : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("02.Lobby");
        }
    }

    void StartStage(BlockOutCharacterType character)
    {
        BlockOutGameData.SelectedCharacter=character;
        BlockOutGameData.CurrentStage=1;
        SceneManager.LoadScene("02.BlockOutGame");
        
    }
    public void SelectHuTao()
    {
        StartStage(BlockOutCharacterType.HuTao);

    }
    public void SelectFurina()
    {
         StartStage(BlockOutCharacterType.Furina);
    }
   public void SelectRaidan()
    {
        StartStage(BlockOutCharacterType.Nilou);
    }

    public void SelectYoimiya()
    {
         StartStage(BlockOutCharacterType.Yoimiya);
    }
}
