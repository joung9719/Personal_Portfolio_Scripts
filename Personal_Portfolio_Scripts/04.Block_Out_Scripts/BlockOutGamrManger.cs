using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockOutGamrManger : MonoBehaviour
{
    public static BlockOutGamrManger Instance;

    [Header("블록 생성 설정")]
    public GameObject blockPrefab;
    public float spacing = 1.1f;
    public float playAreaWidth = 20;
    public float startY = 3.5f;

    [Header("체력")]
    public static int life = 3;
    public Image[] hearts;

    [Header("UI")]
    public static int score = 0;
    public Text scoreText;
    public GameObject gameOverUI;
    public GameObject puseUI;
    public bool isPaused;
    private bool isInstantiate = false;
    public bool stageClearing = false;

    [Header("공 관련")]
    public GameObject ballPrefab;
    public Transform spawnPoint;

    public static int CurrentStage
    {
        get => BlockOutGameData.CurrentStage;
        set => BlockOutGameData.CurrentStage = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    void Start()
    {
        isInstantiate = false;
        stageClearing = false;

        UpDateLifeUI();
        UpDateUI();

        GenerateBlocks(); // 패턴 생성
        SpawnBall();      // 공 생성

        isInstantiate = true;
    }

    void Update()
    {
        HandlePause();

        if (Input.GetKeyDown(KeyCode.P))
        {
            ForcingStageClear();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorApplication.delayCall += () =>
        {
            if (this != null)
                GenerateBlocks();
        };
    }
#endif

    // -------------- 패턴 생성 ---------------

    void GenerateBlocks()
    {
        BlockOutCharacterType ch = BlockOutGameData.SelectedCharacter;
        int stageIndex = BlockOutGameData.CurrentStage - 1;

        if (!BlockOutPattenDB.Patterns.ContainsKey(ch))
        {
            Debug.LogError($"PattenDB에 {ch} 패턴 없음!");
            return;
        }

        int[][,] patterns = BlockOutPattenDB.Patterns[ch];

        if (stageIndex < 0 || stageIndex >= patterns.Length)
        {
            Debug.LogError($"{ch}의 {stageIndex + 1} 스테이지 패턴 없음!");
            return;
        }

        int[,] pattern = patterns[stageIndex];

        // 기존 블록 제거
        foreach (var blk in GameObject.FindGameObjectsWithTag("Block"))
            DestroyImmediate(blk);

        int rows = pattern.GetLength(0);
        int columns = pattern.GetLength(1);

        float blockWidth = (columns - 1) * spacing;
        float startX = -playAreaWidth / 2f + (playAreaWidth - blockWidth) / 2f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (pattern[y, x] == 1)
                {
                    Vector3 pos = transform.position +
                        new Vector3(startX + x * spacing, startY - y * spacing, 0);

                    GameObject b = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                    Block blk = b.GetComponent<Block>();
                    blk.hp = GetRandomHP(BlockOutGameData.CurrentStage);
                }
            }
        }
    }

    int GetRandomHP(int stage)
    {
        if (stage == 1) return 1;
        if (stage == 2) return Random.Range(1, 3);
        if (stage == 3) return Random.Range(1, 4);
        return 1;
    }

    // -------------- 블록 체크 ---------------

    public void CheckBlocks()
    {
        if (!isInstantiate) return;
        if (stageClearing) return;

        int remain = GameObject.FindGameObjectsWithTag("Block").Length;

        if (remain <= 1)
        {
            stageClearing = true;
            BlockOutCharacterType ch = BlockOutGameData.SelectedCharacter;

            BlockOutGameData.clearedCharacters [(int)ch]=true;
            BlockOutGameData.perfectClearCharacters[(int)ch]=BlockOutGameData.isPerfectRun&&life==3;
            string ShowScene = BlockOutStageDB.ShowScenes[ch];
            SceneManager.LoadScene(ShowScene);
        }
    }

    // 개발자 강제 클리어
    public void ForcingStageClear()
    {
        BlockOutCharacterType ch = BlockOutGameData.SelectedCharacter;
        BlockOutGameData.clearedCharacters [(int)ch]=true;
        string ShowScene = BlockOutStageDB.ShowScenes[ch];
        SceneManager.LoadScene(ShowScene);
    }

    // -------------- 공 / UI / Pause ---------------

    void SpawnBall()
    {
        GameObject newBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);

        newBall.GetComponent<BallCltr>().paddle =
            FindObjectOfType<PaddleCtrl>().transform;
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpDateUI();
    }

    public void LoseLife()
    {
        BlockOutGameData.isPerfectRun=false;
        life--;
        UpDateLifeUI();

        if (life > 0)
        {
            SpawnBall();
        }
        else
        {
            gameOverUI.SetActive(true);
        }
    }

    void UpDateLifeUI()
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].gameObject.SetActive(i < life);
    }

    void UpDateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PausedGame();
            else Resume();
        }
    }

    void PausedGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        puseUI.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        puseUI.SetActive(false);
    }

    // -------------- 장면 이동 ---------------

    public void GoSelection()
    {
        Time.timeScale = 1;
        life = 3;
        score = 0;
        isPaused = false;

        foreach (var b in GameObject.FindGameObjectsWithTag("Ball"))
            Destroy(b);
        foreach (var blk in GameObject.FindGameObjectsWithTag("Block"))
            Destroy(blk);

        SceneManager.LoadScene("01.BlockSelection");
    }

    public void ReStart()
    {
        Time.timeScale = 1;
        isPaused = false;

        foreach (var b in GameObject.FindGameObjectsWithTag("Ball"))
            Destroy(b);

        SceneManager.LoadScene("02.BlockOutGame");
    }

    public void ReTry()
    {
        Time.timeScale = 1;
        isPaused = false;

        life = 3;
        score = 0;

        foreach (var blk in GameObject.FindGameObjectsWithTag("Block"))
            Destroy(blk);
        foreach (var bal in GameObject.FindGameObjectsWithTag("Ball"))
            Destroy(bal);

        foreach (var w in GameObject.FindGameObjectsWithTag("Wall"))
            Destroy(w);

        var paddle = GameObject.FindGameObjectWithTag("Paddel");
        if (paddle != null)
            Destroy(paddle);

        BlockOutGameData.CurrentStage = 1;
        BlockOutGameData.isPerfectRun=true;
        SceneManager.LoadScene("02.BlockOutGame");
    }
}
