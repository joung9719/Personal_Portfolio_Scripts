using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using Unity.VisualScripting.Dependencies.NCalc;
using Unity.VisualScripting;

public class QixGameManager : MonoBehaviour
{
    public static QixGameManager instance;
    [Header("UI")]
    public TextMeshProUGUI percentUI;
    public GameObject gameOverUI;
    public GameObject puseUI;

    [Header("플레이어 체력")]
    public static int life=3;
    public UnityEngine.UI.Image[] hearts;
    [Header("스테이지 관련")]
    public CoverMaskInit cover;
    public GameObject enemyPrefab;
    public Transform[] enemySpawnPoints;
    [Header("클리어 조건")]
    public float clearPercent=80f;   
    [Header("피격 판정")]
    private bool isHit=false;
    public float hitCooldown=1f;
    private bool isPaused=false;


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        LoadStage();
        UpDateLifeUI();
    }

    void Update()
    {
        CheckPauseKey();
        if(Input.GetKeyDown(KeyCode.P))
        {
            NextStage();
        }
    }

    void LoadStage()
    {
        var info=QixStageDB.stages[QixGameData.currentCharater][QixGameData.CurrentStage];
        cover.background.sprite=info.background;
        SpawnEnemise(info.enemyCount,info.enemySpeed);
    }

    void SpawnEnemise(int count,float speed)
    {
        for(int i=0;i<count;i++)
        {
            var enemy=Instantiate(enemyPrefab,enemySpawnPoints[i].position,Quaternion.identity);
            var ctrl=enemy.GetComponent<Enemy_Ctrl>();
            ctrl.moveSpeed=speed;
        }
    }

    public void UpdatePercent()
    {
        float p = cover.GetRemovedPercent();
        percentUI.text = $"{p:0.0}%";
        if(p>=clearPercent)
        {
            NextStage();
        }
    }

    public void NextStage()
    {
       QixGameData.CurrentStage++;
       int maxStages=QixStageDB.stages[QixGameData.currentCharater].Length;

       if(QixGameData.CurrentStage>=maxStages)
        {
            QixGameData.clearCharaters[(int)QixGameData.currentCharater]=true;
            QixGameData.perfectClearCharacters[(int)QixGameData.currentCharater]=QixGameData.isPerfectRun&&life==3;
           switch(QixGameData.currentCharater)
            {
                case QixCharacterType.Sora:
                SceneManager.LoadScene("SoraStageClear04");
                break;
                case QixCharacterType.D:
                SceneManager.LoadScene("DStageClear04");
                break;
                case QixCharacterType.Nayuta:
                SceneManager.LoadScene("NayutaStageClear04");
                break;
                case QixCharacterType.Liberialo:
                SceneManager.LoadScene("LibealioStageClear04");
                break;
            }
            return;
        }
        SceneManager.LoadScene("02.QixGame");
    }

    public void PlayerHit()
    {
        if(isHit)return;

        StartCoroutine(HitDelay());
        QixGameData.isPerfectRun=false;
        life--;
        UpDateLifeUI();
        if(life<=0)
        {
            GameOver();
        }
        else
        {
            Restart();
        }
    }

    IEnumerator HitDelay()
    {
        isHit=true;
        yield return new WaitForSeconds(hitCooldown);
        isHit=false;
    }
    void UpDateLifeUI()
    {
        for(int i=0;i<hearts.Length;i++)
        {
            hearts[i].gameObject.SetActive(i<life);
        }
    }

    public void GameOver()
    {
       Time.timeScale=0;
       gameOverUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("02.QixGame");
       
    }

   void CheckPauseKey()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            PauseGame();
            else
            ResumGame();
        }
    }

    void PauseGame()
    {
        isPaused=true;
        Time.timeScale=1;
        puseUI.SetActive(true);
    }

    public void ResumGame()
    {
        isPaused=false;
        Time.timeScale=1;

        if(puseUI!=null)
        puseUI.SetActive(false);
        if(gameOverUI!=null)
        gameOverUI.SetActive(false);
    }

    public void GoSelection()
    {
       Time.timeScale=1;
       life=3;
       isPaused=false;

       foreach(var cut in GameObject.FindGameObjectsWithTag("CutArea"))
       Destroy(cut);

       SceneManager.LoadScene("01.QixSelection");
    }

    

    public void ReTry()
    {
        Time.timeScale=1;
        QixGameData.CurrentStage=0;
        life=3;
        QixGameData.isPerfectRun=true;
        SceneManager.LoadScene("02.QixGame");

       
    }
}
