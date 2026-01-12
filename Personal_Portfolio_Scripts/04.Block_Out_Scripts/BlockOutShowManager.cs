using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlockOutShowManager : MonoBehaviour
{
    public Image showImage;

    private void Start()
    {
        RemoveCoverByStage();
        StartCoroutine(ShowRoutine());
    }



    void RemoveCoverByStage()
    {
        int stage = BlockOutGameData.CurrentStage;

        if (stage >= 1)
            DestroyByTag("Cover01");
        if (stage >= 2)
            DestroyByTag("Cover02");
        if (stage >= 3)
            DestroyByTag("Cover03");

    }
    void DestroyByTag(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj != null)
            Destroy(obj);
    }

    IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(1.2f);

       BlockOutCharacterType ch=BlockOutGameData.SelectedCharacter;
       int maxStage=BlockOutStageDB.MaxStage[ch];

        if (BlockOutGameData.CurrentStage >= maxStage)
        {
            BlockOutGameData.clearedCharacters[(int)ch]=true;
            string clearScene=BlockOutStageDB.ClearScenes[ch];
            SceneManager.LoadScene(clearScene);
        }
        else
        {
            BlockOutGameData.CurrentStage++;
            SceneManager.LoadScene("02.BlockOutGame");
        }
    }
}
