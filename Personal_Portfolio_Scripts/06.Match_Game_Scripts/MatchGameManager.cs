using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

class AStarNode
{
    public int r, c;
    public int g, h;
    public int f => g + h;
    public AStarNode parent;
    public AStarNode(int r, int c)
    {
        this.r = r;
        this.c = c;
    }
}

public class MatchGameManager : MonoBehaviour
{
    public static MatchGameManager Instance;
    [Header("배경이미지")]
    [SerializeField]
    private Image backGroundImage;
    [Header("타일 및 보드")]
    public GameObject tilePreFab;
    public Transform tileParent;
    public Sprite[] tileSprites;
    public int rows = 12;
    public int cols = 10;
    [Header("타일 크기")]
    public float cellW = 40f;
    public float cellH = 40f;
    [Header("보드 배열")]
    public Tile[,,] board;
    public int CurrentStage;
    private int currentLayer;
    private Tile fristTile;
    private Tile secondTile;
    [Header("A*이용해서 블럭 끼리 라인 연결?")]
    LineRenderer lr;
    public Material lineMaterial;
    public Canvas mainCanvas;

    [SerializeField] RectTransform tileMask;
    [SerializeField] RectTransform lineHolder;
    [Header("타일 선택")]
    public Tile selectedTile = null;
    [Header("클리어조건")]
    public int totalTileCount;
    public int removeTileCount = 0;
    [Header("UI")]
    public Text timeText;
    public Image[] lifeImges;
    public Text scoreText;
    public Text round;
    private float timer;
    public GameObject puseUI;
    public GameObject gameOverUI;
    [Header("층 표시")]
    [SerializeField]
    private bool hideLayer=true;
    private int maxLayerCount;
    [Header("배경 숨김")]
    [SerializeField]private CanvasGroup backImageGroup;
    [SerializeField]private int ShowBackLayer=0;

    private bool isPaused = false;

    public int CurrentLayer => currentLayer;




    private void Awake()
    {
        Instance = this;

        lr = GameObject.Find("LineRendererHolder").GetComponent<LineRenderer>();

        lr.useWorldSpace = false;
        lr.widthMultiplier = 0.05f;
        lr.material = lineMaterial;
        lr.startColor = Color.black;
        lr.endColor = Color.black;
        lr.sortingOrder = 999;

    }
    // Start is called before the first frame update
    private void Start()
    {
        CurrentStage = MatchGameGameData.CurrentStage;
        currentLayer = CurrentStage;
        maxLayerCount=currentLayer+1;
        Debug.Log("현재 스테이지 = " + currentLayer + " (1층부터 " + (currentLayer + 1) + "층까지)");
        lineHolder.SetParent(tileMask);
        lineHolder.anchorMin = new Vector2(0, 1);
        lineHolder.anchorMax = new Vector2(0, 1);
        lineHolder.pivot = new Vector2(0, 1);
        lineHolder.anchoredPosition = Vector2.zero;
        lineHolder.localScale = Vector3.one;

        int maxRows = 0;
        int maxCols = 0;
        int maxLayer = currentLayer + 1;

        for (int layer = 0; layer < maxLayer; layer++)
        {
            int[,] pattern = MatchGamePatternDB.StagePatterns[MatchGameGameData.SelectedCharacter][layer];
            maxRows = Mathf.Max(maxRows, pattern.GetLength(0));
            maxCols = Mathf.Max(maxCols, pattern.GetLength(1));
        }

        rows = maxRows;
        cols = maxCols;

        board = new Tile[3, rows, cols];
        CreateTiles();
        InitTile();
        if(hideLayer)
        ApplyLayerVisibility();
        timer = MatchGameGameData.remainTime;
        UpdateUI();
        LoadBackGround();
        AutoShuffelIfNoMove();
    }
    void Update()
    {
        timer -= Time.deltaTime;
        MatchGameGameData.remainTime = timer;

        if (timer <= 0)
        {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GoNextStage();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PuseGame();
        }
        if (isPaused)
            return;
        UpdateUI();
    }

    void UpdateUI()
    {
        int m = Mathf.FloorToInt(timer / 60);
        int s = Mathf.FloorToInt(timer % 60);
        timeText.text = $"{m:00}:{s:00}";
        scoreText.text = $"SCORE:{MatchGameGameData.score}";
        round.text = $"ROUND:{MatchGameGameData.round}";

        UpdateLifeUI();
    }

    void UpdateLifeUI()
    {
        int life = MatchGameGameData.life;
        for (int i = 0; i < lifeImges.Length; i++)
        {
            if (i < life)
                lifeImges[i].gameObject.SetActive(true);
            else
                lifeImges[i].gameObject.SetActive(false);
        }
    }

    public void AddSocre(int amount)
    {
        MatchGameGameData.score += amount;
        UpdateUI();
    }
    public void LoseLife()
    {
        MatchGameGameData.isPerfectRun=false;
        MatchGameGameData.life--;
        if (MatchGameGameData.life <= 0)
        {
            GameOver();
            return;
        }

        UpdateUI();
    }
    void GameOver()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
    }
    public void PuseGame()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            puseUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            puseUI.SetActive(false);
        }
    }


   void CreateTiles()
{
    int maxLayer = currentLayer + 1;

    // 1) 패턴에서 실제로 생성될 "좌표 목록"을 먼저 만든다 (짝이 무조건 맞게 하기 위함)
    List<(int layer, int r, int c)> spawnPositions = new List<(int, int, int)>();

    for (int layer = 0; layer < maxLayer; layer++)
    {
        int[,] pattern = MatchGamePatternDB.StagePatterns[MatchGameGameData.SelectedCharacter][layer];
        int pr = pattern.GetLength(0);
        int pc = pattern.GetLength(1);

        for (int r = 0; r < pr; r++)
        {
            for (int c = 0; c < pc; c++)
            {
                if (pattern[r, c] == 1)
                    spawnPositions.Add((layer, r, c));
            }
        }
    }

    // 2) 홀수면 한 칸은 아예 비워서 "타일 개수" 자체를 짝수로 만든다
    if (spawnPositions.Count % 2 == 1)
    {
        int removeIndex = Random.Range(0, spawnPositions.Count);
        var removed = spawnPositions[removeIndex];
        spawnPositions.RemoveAt(removeIndex);
        Debug.LogWarning($"[MatchGame] 패턴 타일 개수가 홀수여서 1칸 비웠음 -> layer:{removed.layer}, r:{removed.r}, c:{removed.c}");
    }

    // 3) 남은 타일 개수만큼, 무조건 2개씩 짝이 되도록 ID를 만든다
    List<int> allTileIDs = new List<int>();
    int totalTiles = spawnPositions.Count;

    for (int i = 0; i < totalTiles / 2; i++)
    {
        int spriteID = i % tileSprites.Length;
        allTileIDs.Add(spriteID);
        allTileIDs.Add(spriteID);
    }

    // 셔플
    for (int i = 0; i < allTileIDs.Count; i++)
    {
        int rand = Random.Range(i, allTileIDs.Count);
        (allTileIDs[i], allTileIDs[rand]) = (allTileIDs[rand], allTileIDs[i]);
    }

    // 4) 좌표 목록에 따라 타일 생성 + ID 배정
    for (int i = 0; i < spawnPositions.Count; i++)
    {
        var pos = spawnPositions[i];

        GameObject obj = Instantiate(tilePreFab, tileParent);
        Tile tile = obj.GetComponent<Tile>();

        tile.row = pos.r;
        tile.col = pos.c;
        tile.layer = pos.layer;

        tile.tileID = allTileIDs[i];
        tile.SetImage(tileSprites[tile.tileID]);

        RectTransform rect = obj.GetComponent<RectTransform>();
        float layerOffsetX = pos.layer;
        float layerOffsetY = pos.layer;
        rect.anchoredPosition = new Vector2(pos.c * cellW + layerOffsetX, -pos.r * cellH + layerOffsetY);

        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = obj.AddComponent<CanvasGroup>();

        board[pos.layer, pos.r, pos.c] = tile;
    }
}


    public void SelectTile(Tile tile)
    {
        if (fristTile == null)
        {
            Debug.Log("선택");
            fristTile = tile;
            return;
        }

        if (secondTile == null)
        {
            secondTile = tile;

            CheckMatch();
        }
    }

    void CheckMatch()
    {
        Debug.Log($"매칭 체크->First:Layer{fristTile.layer},Index{fristTile.row},{fristTile.col},ID{fristTile.tileID}/" +
                  $"Second:Layer{secondTile.layer},Index{secondTile.row},{secondTile.col},ID{secondTile.tileID}");
        if (fristTile.tileID == secondTile.tileID)
        {
            if (CanConnect(fristTile, secondTile))
            {

                List<Vector2Int> path = FindAStarPath(fristTile, secondTile);
                if (path != null)
                {
                    DrawAStarLine(path);
                }
                Debug.Log(">>>매칭 성공 ->제거");
                board[fristTile.layer, fristTile.row, fristTile.col] = null;
                board[secondTile.layer, secondTile.row, secondTile.col] = null;

                fristTile.Remove();
                secondTile.Remove();
                AddSocre(100);
            }
            else
            {
                Debug.Log("경로 없음");
            }
        }
        else
        {
            Debug.Log("매칭 실패");
            LoseLife();
        }

        fristTile = null;
        secondTile = null;
        AutoShuffelIfNoMove();

    }

    readonly int[] dr = { 0, 0, 1, -1 };
    readonly int[] dc = { 1, -1, 0, 0 };

    bool CanConnect(Tile a, Tile b)
    {
        if (a.tileID != b.tileID)
            return false;

        int h = rows + 2;
        int w = cols + 2;


        int[,,] visited = new int[h, w, 4];
        for (int r = 0; r < h; r++)
        {
            for (int c = 0; c < w; c++)
            {
                for (int d = 0; d < 4; d++)
                    visited[r, c, d] = 999;
            }
        }

        int sr = a.row + 1;
        int sc = a.col + 1;
        int tr = b.row + 1;
        int tc = b.col + 1;


        Queue<Node> q = new Queue<Node>();
        q.Enqueue(new Node(sr, sc, -1, 0));

        while (q.Count > 0)
        {
            Node cur = q.Dequeue();

            for (int dir = 0; dir < 4; dir++)
            {
                int nr = cur.r + dr[dir];
                int nc = cur.c + dc[dir];

                if (nr < 0 || nr >= h || nc < 0 || nc >= w)
                    continue;

                int newTurns = cur.dir == -1 || cur.dir == dir ? cur.turns : cur.turns + 1;
                if (newTurns > 2)
                    continue;


                if (visited[nr, nc, dir] <= newTurns)
                    continue;

                if (nr == tr && nc == tc)
                {
                    return true;
                }

                if (IsBlockedExpanded(nr, nc, a, b))
                    continue;

                visited[nr, nc, dir] = newTurns;
                q.Enqueue(new Node(nr, nc, dir, newTurns));
            }
        }

        return false;
    }

    bool IsBlockedExpanded(int er, int ec, Tile a, Tile b)
    {
        if (er >= 1 && er <= rows && ec >= 1 && ec <= cols)
        {
            Tile t = board[a.layer, er - 1, ec - 1];
            if (t == null || t == a || t == b || !t.gameObject.activeSelf)
                return false;
            return true;
        }

        return false;
    }

    struct Node
    {
        public int r, c;
        public int dir;
        public int turns;

        public Node(int r, int c, int dir, int turns)
        {
            this.r = r;
            this.c = c;
            this.dir = dir;
            this.turns = turns;
        }
    }

    List<Vector2Int> FindAStarPath(Tile a, Tile b)
    {
        int H = rows + 2;
        int W = cols + 2;

        AStarNode[,] nodes = new AStarNode[H, W];

        for (int r = 0; r < H; r++)
        {
            for (int c = 0; c < W; c++)
            {
                nodes[r, c] = new AStarNode(r, c);
            }
        }
        int sr = a.row + 1;
        int sc = a.col + 1;
        int tr = b.row + 1;
        int tc = b.col + 1;

        List<AStarNode> open = new List<AStarNode>();
        HashSet<AStarNode> colsed = new HashSet<AStarNode>();

        AStarNode start = nodes[sr, sc];
        AStarNode end = nodes[tr, tc];

        start.g = 0;
        start.h = Mathf.Abs(sr - tr) + Mathf.Abs(sc - tc);
        open.Add(start);

        int[,] dir = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

        while (open.Count > 0)
        {
            open.Sort((x, y) => x.f.CompareTo(y.f));

            AStarNode cur = open[0];
            open.RemoveAt(0);
            colsed.Add(cur);

            if (cur == end)
                return RetraceAStar(cur);

            for (int i = 0; i < 4; i++)
            {
                int nr = cur.r + dir[i, 0];
                int nc = cur.c + dir[i, 1];

                if (nr < 0 || nr >= H || nc < 0 || nc >= W)
                    continue;
                if (IsBlockedExpanded(nr, nc, a, b))
                    continue;

                AStarNode next = nodes[nr, nc];
                if (colsed.Contains(next))
                    continue;

                int newG = cur.g + 1;

                if (!open.Contains(next) || newG < next.g)
                {
                    next.g = newG;
                    next.h = Mathf.Abs(nr - tr) + Mathf.Abs(nc - tc);
                    next.parent = cur;

                    if (!open.Contains(next))
                        open.Add(next);
                }
            }
        }
        return null;
    }
    List<Vector2Int> RetraceAStar(AStarNode node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        AStarNode cur = node;

        while (cur != null)
        {
            path.Add(new Vector2Int(cur.r, cur.c));
            cur = cur.parent;
        }

        path.Reverse();
        return path;
    }
    Vector3 AStarToWorld(int r, int c)
    {
        int rr = r - 1;
        int cc = c - 1;

        if (rr >= 0 && rr < rows && cc >= 0 && cc < cols)
        {
            Tile t = board[currentLayer, rr, cc];
            if (t != null)
            {
                RectTransform rect = t.GetComponent<RectTransform>();
                return UIToWorld(rect);
            }

        }

        RectTransform parentRect = tileParent.GetComponent<RectTransform>();
        Vector3 parentWorld = UIToWorld(parentRect);


        float scale = mainCanvas.scaleFactor;
        float x = parentWorld.x + (cc * (cellW / scale));
        float y = parentWorld.y - (rr * (cellH / scale));

        return new Vector3(x, y, 0);


    }
    Vector3 UIToWorld(RectTransform rect)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, rect.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }
    Vector3 AStarToUIPos(int r, int c)
    {
        float x = c * cellW + (cellW / 2f);
        float y = -r * cellH - (cellH / 2f);

        return new Vector3(x, y, 0);
    }
    bool IsBlockForAStar(int er, int ec, Tile a, Tile b)
    {
        if (er >= 1 && er <= rows && ec >= 1 && ec <= cols)
        {
            Tile t = board[currentLayer, er - 1, ec - 1];
            if (t == null || t == a || t == b)
                return false;
            return true;
        }
        return false;
    }

    public void SelectedTile(Tile tile)
    {
        if (selectedTile != null)
            selectedTile.SetHighlight(false);

        selectedTile = tile;
        selectedTile.SetHighlight(true);
    }
    void DrawAStarLine(List<Vector2Int> path)
    {
        lr.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pos = AStarToUIPos(path[i].x - 1, path[i].y - 1);
            lr.SetPosition(i, pos);
        }

        Invoke(nameof(ClearLine), 2f);
    }

    void ClearLine()
    {
        lr.positionCount = 0;
    }

    void InitTile()
    {
        int maxLayer = currentLayer + 1;
        totalTileCount = 0;

        for (int layer = 0; layer < maxLayer; layer++)
        {
            int[,] pattern = MatchGamePatternDB.StagePatterns[MatchGameGameData.SelectedCharacter][layer];

            for (int r = 0; r < pattern.GetLength(0); r++)
            {
                for (int c = 0; c < pattern.GetLength(1); c++)
                {
                    if (pattern[r, c] == 1)
                        totalTileCount++;
                }
            }
        }
        removeTileCount = 0;
    }

    public void OnTileRemoved()
    {
        removeTileCount++;

        if (IsCurrentLayerCleared())
        {
            currentLayer--;
            if(hideLayer&&currentLayer>=0)
            {
                ApplyLayerVisibility();
            }
            if (currentLayer < 0)
            {
                GoNextStage();
            }
        }
        AutoShuffelIfNoMove();


    }
    bool IsCurrentLayerCleared()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Tile tile = board[currentLayer, r, c];
                if (tile != null && tile.gameObject.activeSelf)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void GoNextStage()
    {
        MatchGameGameData.CurrentStage++;

        if (MatchGameGameData.CurrentStage >= 3)
        {

            MatchGameCharacter ch = MatchGameGameData.SelectedCharacter;
            MatchGameGameData.clearCharater[(int)ch] = true;
            MatchGameGameData.perfectClearCharacters[(int)ch]=MatchGameGameData.isPerfectRun&&MatchGameGameData.life==3;
            string clearScene = MatchGameStageDB.ClearScenes[ch];

            SceneManager.LoadScene(clearScene);
            return;
        }
        SceneManager.LoadScene("02.MatchGame");
    }

    void LoadBackGround()
    {
        var ch = MatchGameGameData.SelectedCharacter;
        int stage = MatchGameGameData.CurrentStage;

        if (!MatchGameStageDB.StageScenes.ContainsKey(ch))
        {
            Debug.LogError($"[MatchGame] {ch} 캐릭터 이미지가 StageDB에 없음!");
            return;
        }

        string[] paths = MatchGameStageDB.StageScenes[ch];

        if (stage >= paths.Length)
        {
            Debug.LogError("[MatchGame] 스테이지 번호가 이미지 개수보다 큼!");
            return;
        }

        string imgPath = paths[stage];
        Sprite sprite = Resources.Load<Sprite>(imgPath);

        if (sprite == null)
        {
            Debug.LogError($"[MatchGame] 이미지 로드 실패: {imgPath}");
            return;
        }

        backGroundImage.sprite = sprite;
    }

    public void ReStart()
    {
        Time.timeScale=1f;
        isPaused=false;

        if(puseUI!=null)
        puseUI.SetActive(false);
        if(gameOverUI!=null)
        gameOverUI.SetActive(false);

        fristTile=null;
        secondTile=null;

        if(selectedTile!=null)
        {
            secondTile.SetHighlight(false);
            selectedTile=null;
        }

        CancelInvoke(nameof(ClearLine));
        ClearLine();

        MatchGameGameData.remainTime=900f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReTry()
    {
        Time.timeScale=1f;
        isPaused=false;

        if(puseUI!=null)
        puseUI.SetActive(false);
        if(gameOverUI!=null)
        gameOverUI.SetActive(false);

        MatchGameGameData.CurrentStage=0;
        MatchGameGameData.life=3;
        MatchGameGameData.score=0;
        MatchGameGameData.round=0;
        MatchGameGameData.remainTime=900f;

        CancelInvoke(nameof(ClearLine));
        ClearLine();
        MatchGameGameData.isPerfectRun=true;
        SceneManager.LoadScene("02.MatchGame");
    }
    public void GoSeletion()

    {
        SceneManager.LoadScene("01.MatchGameSelection");
    }
    private void ApplyLayerVisibility()
    {
        for (int layer=0;layer<maxLayerCount;layer++)
        {
            bool visible=(layer==currentLayer);
            for(int r=0;r<rows;r++)
            {
                for(int c=0;c<cols;c++)
                {
                    Tile t=board[layer,r,c];
                    if(t==null)
                    continue;
                    CanvasGroup cg=t.GetComponent<CanvasGroup>();
                    if(cg==null)
                    cg=t.gameObject.AddComponent<CanvasGroup>();

                    cg.alpha=visible?1f:0f;
                    cg.blocksRaycasts=visible;
                    cg.interactable=visible;
                    
                }
            }
        }
        if(backImageGroup!=null)
        {
            bool bgVisible=(currentLayer<=ShowBackLayer);
            backImageGroup.alpha=bgVisible?1f:0f;
            backImageGroup.blocksRaycasts=bgVisible;
            backImageGroup.interactable=bgVisible;
        }
    }

    void AutoShuffelIfNoMove()
    {
        if(currentLayer<0)
        return;
        if(!HasAnyAvailableMatch(currentLayer))
        {
            ShuffleCurrentLayerGuaranteed();
        }
    }

    bool HasAnyAvailableMatch(int layer)
    {
        List<Tile>tiles=GetActiveTiles(layer);
        for(int i=0;i<tiles.Count;i++)
        {
            for(int j=i+1;j<tiles.Count;j++)
            {
                if(tiles[i].tileID!=tiles[j].tileID)
                continue;
                if(CanConnect(tiles[i],tiles[j]))
                return true;
            }
        }
        return false;
    }
    List<Tile>GetActiveTiles(int layer)
    {
        List<Tile>result=new List<Tile>();
        if(layer<0)
        return result;

        for(int r=0;r<rows;r++)
        {
            for(int c=0;c<cols;c++)
            {
                Tile t=board[layer,r,c];
                if(t!=null&&t.gameObject.activeSelf)
                result.Add(t);
            }
        }
        return result;
    }

    void ShuffleCurrentLayerGuaranteed()
    {
        int layer=currentLayer;
        List<Tile> tiles=GetActiveTiles(layer);
        if(tiles.Count<2)
        return;

        Tile slotA,slotB;
        if(!TryFindEasyConnectPair(layer,out slotA,out slotB))
        {
            return;
        }

        List<int>originIds=new List<int>();
        Dictionary<int ,int>counts=new Dictionary<int, int>();


        for(int i=0;i<tiles.Count;i++)
        {
            int id=tiles[i].tileID;
            originIds.Add(id);

            if(!counts.ContainsKey(id))
            counts.Add(id,0);
            counts[id]++;
        }

        int chosenId=originIds[0];
        foreach(var kv in counts)
        {
            if(kv.Value>=2)
            {
                chosenId=kv.Key;
                break;
            }
        }

        const int MAX_TRY=20;

        for(int attempt=0;attempt<MAX_TRY;attempt++)
        {
            List<int> work=new List<int>(originIds);

            work.Remove(chosenId);
            work.Remove(chosenId);
            ShuffleIntList(work);

            int idx=0;
            for(int i=0;i<tiles.Count;i++)
            {
                Tile t=tiles[i];
                if(t==slotA||t==slotB)
                continue;

                int id=work[idx++];
                t.tileID=id;
                t.SetImage(tileSprites[id]);
            }

            slotA.tileID=chosenId;
            slotA.SetImage(tileSprites[chosenId]);

            slotB.tileID=chosenId;
            slotB.SetImage(tileSprites[chosenId]);

            CancelInvoke(nameof(ClearLine));
            ClearLine();

            if(HasAnyAvailableMatch(layer))
            return;
        }

        void ShuffleIntList(List<int>list)
        {
            for(int i=0;i<list.Count;i++)
            {
                int rand=Random.Range(i,list.Count);
                (list[i],list[rand])=(list[rand],list[i]);
            }
        }

        bool TryFindEasyConnectPair(int layer,out Tile a,out Tile b)
        {
            a=null;
            b=null;

            for(int r=0;r<rows;r++)
            {
                for(int c=0;c<cols;c++)
                {
                    Tile t=board[layer,r,c];
                    if(t==null||!t.gameObject.activeSelf)
                    continue;

                    if(c+1<cols)
                    {
                        Tile right=board[layer,r,c+1];
                        if(right!=null&&right.gameObject.activeSelf)
                        {
                            a=t;b=right;
                            return true;
                        }
                    }

                    if(r+1<rows)
                    {
                        Tile down=board[layer,r+1,c];
                        if(down!=null&&down.gameObject.activeSelf)
                        {
                            a=t;b=down;
                            return true;
                        }
                    }
                }
            }
            List<Tile>tiles=GetActiveTiles(layer);
            for(int i=0;i<tiles.Count;i++)
            {
                for(int j=i+1;j<tiles.Count;j++)
                {
                    if(CanConnectIgnoringId(tiles[i],tiles[j]))
                    {
                        a=tiles[i];
                        b=tiles[j];
                        return true;
                    }
                }
            }
            return false;
        }

        bool CanConnectIgnoringId(Tile a,Tile b)
        {
            int h= rows+2;
            int w=cols+2;

            int [,,]visited=new int [h,w,4];
            for(int r=0;r<h;r++)
            for(int c=0;c<w;c++)
            for(int d=0;d<4;d++)
            visited[r,c,d]=999;

            int sr=a.row+1;
            int sc=a.col+1;
            int tr=b.row+1;
            int tc=b.col+1;

            Queue<Node>q=new Queue<Node>();
            q.Enqueue(new Node(sr,sc,-1,0));

            while(q.Count>0)
            {
                Node cur=q.Dequeue();
                for(int dir=0;dir<4;dir++)
                {
                    int nr=cur.r+dr[dir];
                    int nc=cur.c+dc[dir];

                    if(nr<0||nr>=h||nc<0||nc>=w)
                    continue;

                    int newTurns=cur.dir==-1||cur.dir==dir?cur.turns:cur.turns+1;
                    if(newTurns>2)
                    continue;

                    if(visited[nr,nc,dir]<=newTurns)
                    continue;

                    if(nr==tr&&nc==tc)
                    return true;

                    if(IsBlockedExpanded(nr,nc,a,b))
                    continue;

                    visited[nr,nc,dir]=newTurns;
                    q.Enqueue(new Node(nr,nc,dir,newTurns));
                }
            }
            return false;
        }
    }





}
