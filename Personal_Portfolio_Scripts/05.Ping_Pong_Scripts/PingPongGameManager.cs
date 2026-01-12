using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class PingPongGameManager : MonoBehaviour
{

    public static PingPongGameManager Instance;
    [Header("UI")]
    public Text txtPlayerScore;
    public Text txtEnemyScore;
    public Text txtLife;
    public Text txtRound;
    public GameObject gameOverUI;
    public GameObject puseUI;
    public Image[] heatImages;

    public bool isPaused;
    [Header("점수 관련")]
    public int playerScore=0;
    public int enemyScore=0;
    public int winScore=2;
    public static int life=3;
   
    [Header("공 스폰")]
    public GameObject ballPrefab;
    public Transform playerSpawn;
    public Transform enemySpawn;
    
   

    private void Awake()
    {
        Instance=this;
    }
    void Start()
    {
        UpdateLifeUI();
        txtRound.text=$"Round:{PingPongGameDate.CurrentStage}";
        SpawnBall(false);
    }

    void Update()
    {
      HandlePause();
        if(Input.GetKeyDown(KeyCode.P))
        {
            NextStage();
        }
    }

    public void AddScore(bool isplayer)
    {
        if(isplayer)
        {
            playerScore++;
            txtPlayerScore.text=playerScore.ToString();

           SpawnBall(true);
        }
        else
        {
            enemyScore++;

            txtEnemyScore.text=enemyScore.ToString();
            SpawnBall(false);
        }
        
        if(playerScore>=winScore)
        {
            winRound(); 
        }
        else if(enemyScore>=winScore)
        {
            LoseRound();
        }
    }

    void winRound()
    {
        var ch=PingPongGameDate.SelectedCharacter;
        if(PingPongGameDate.CurrentStage>=3)
        {
            PingPongGameDate.clearCharaters[(int)ch]=true;
            PingPongGameDate.perfectClearCharacters[(int)ch]=PingPongGameDate.isPerfectRun&&life==3;
        }
        SceneManager.LoadScene(PingPongStageDB.ShowScene[ch]);
    }

    void LoseRound()
    {
        PingPongGameDate.isPerfectRun=false;
        life--;
       UpdateLifeUI();
        if(life<=0)
        {
            
            gameOverUI.SetActive(true);
        }
        else
        {
            ReStartRound();
        }
    }

    public void ReStartRound()
    {
        Time.timeScale=1;
        isPaused=false;

        foreach(var ball in GameObject.FindGameObjectsWithTag("Ball"))
        Destroy(ball);
        PingPongCharacter ch=PingPongGameDate.SelectedCharacter;
        int idx=Mathf.Clamp(PingPongGameDate.CurrentStage-1,0,2);

        string restartScene=PingPongStageDB.StageScenes[ch][idx];
        SceneManager.LoadScene(restartScene); 
       

    }

    public void SpawnBall(bool towardsPlayer)
    {
        Vector3 pos;
        int dir;
        if(towardsPlayer)
        {
            pos=enemySpawn.position;
            dir=-1;
        }
        else
        {
            pos=playerSpawn.position;
            dir=1;
        }

       var ballObj=Instantiate(ballPrefab,pos,Quaternion.identity);
       var ballCtr=ballObj.GetComponent<PingPongBallCtr>();
       ballCtr.ResetBall(PingPongGameDate.CurrentStage,pos,dir);
    }

    public void UpdateLifeUI()
    {
        for(int i=0;i<heatImages.Length;i++)
        {
            heatImages[i].enabled=(i<life);
        }
    }

    public void NextStage()
    {
       PingPongCharacter ch = PingPongGameDate.SelectedCharacter;
        string showScene = PingPongStageDB.ShowScene[ch];
        SceneManager.LoadScene(showScene); 
    }

    void HandlePause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            PauseGame();
            else
            ResumeGame();
        }
    }

    void PauseGame()
    {
        isPaused=true;
        Time.timeScale=0;
        puseUI.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused=false;
        Time.timeScale=1;
        puseUI.SetActive(false);
    }

    

    public void GoPingPongSelection()
    {
        Time.timeScale=1;
        isPaused=false;
        life=3;
        playerScore=0;
        enemyScore=0;
        foreach(var ball in GameObject.FindGameObjectsWithTag("Ball"))
        Destroy(ball);

        if(puseUI!=null)
        puseUI.SetActive(false);
        if(gameOverUI!=null)
        gameOverUI.SetActive(false);

       SceneManager.LoadScene("01.PingPongSelection"); 
    }

    public void ReTry()
    {
        Time.timeScale=1;
        isPaused=false;
        life=3;
        playerScore=0;
        enemyScore=0;

        UpdateLifeUI();

        foreach(var ball in GameObject.FindGameObjectsWithTag("Ball"))
        Destroy(ball);

        PingPongCharacter ch=PingPongGameDate.SelectedCharacter;
       
        string restartScene=PingPongStageDB.StageScenes[ch][0];
        PingPongGameDate.isPerfectRun=false;

        SceneManager.LoadScene(restartScene);
    }



}
