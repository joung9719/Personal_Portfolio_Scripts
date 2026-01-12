using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GalleryGameTyp
{
    Qix,
    BlockOut,
    PingPong,
    MatchGame
}

[System.Serializable]
public class CharaterGallery
{
    public string CharaterName;
    public Sprite[] images;
    public int unlockIndex = -1;
}

public class GallertManager : MonoBehaviour
{
    public static GallertManager Instance;

    [Header("각 게임 갤러리 자동 할당")]
    public GalleryGameTyp galleryGameTyp = GalleryGameTyp.Qix;
    public bool autoDetectByScenName = true;

    [Header("갤러리 데이터")]
    public CharaterGallery[] galleryList;

    [Header("UI")]
    public Image displayImage;
    public Text charaterNameText;

    [Header("캐릭터 넘기기 버튼")]
    public Button leftBtn;
    public Button rigntBtn;

    [Header("이미지 넘기기 버튼(추가)")]
    public Button imageLeftBtn;
    public Button imageRightBtn;

    [Header("잠금 이미지")]
    public Sprite lockedSprite;
    [Header("애니매이션")]
    public float fadeDuration = 0.15f;
    private CanvasGroup displayGroup;
    private Coroutine fadeCo;
    private Sprite _lastSprite;

    private int currentCharater = 0;
    private int currentImage = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Bind(Button btn, UnityEngine.Events.UnityAction act)
    {
        if (btn == null)
            return;
        btn.onClick = new Button.ButtonClickedEvent();
        btn.onClick.AddListener(act);
    }

    void Start()
    {
        if (autoDetectByScenName)
            AutoDetectGameTpyeBySceneName();
        Bind(leftBtn, PreniousCharater);
        Bind(rigntBtn, NextCharater);

        Bind(imageLeftBtn, PreviousImage);
        Bind(imageRightBtn, NextImage);
        if (displayGroup != null)
        {
            displayGroup = displayImage.GetComponent<CanvasGroup>();
            if (displayGroup == null)
                displayGroup = displayGroup.gameObject.AddComponent<CanvasGroup>();
            displayGroup.alpha = 1f;
        }

        UpdateGallery();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("03.Gallery");
        }
    }

    void AutoDetectGameTpyeBySceneName()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene.Contains("Qix", StringComparison.OrdinalIgnoreCase))
            galleryGameTyp = GalleryGameTyp.Qix;
        else if (scene.Contains("BlockOut", StringComparison.OrdinalIgnoreCase))
            galleryGameTyp = GalleryGameTyp.BlockOut;
        else if (scene.Contains("PingPong", StringComparison.OrdinalIgnoreCase))
            galleryGameTyp = GalleryGameTyp.PingPong;
        else if (scene.Contains("MatchGame", StringComparison.OrdinalIgnoreCase))
            galleryGameTyp = GalleryGameTyp.MatchGame;
    }

    int GetUnlockIndex()
    {
        if (galleryList == null || galleryList.Length == 0) return -1;
        var data = galleryList[currentCharater];
        return (data != null && data.unlockIndex >= 0) ? data.unlockIndex : currentCharater;
    }

    bool IsUnlockedByIndex(int unlockIdx)
    {
        if (unlockIdx < 0) return false;

        switch (galleryGameTyp)
        {
            case GalleryGameTyp.Qix:
                return (QixGameData.clearCharaters != null &&
                        unlockIdx >= 0 && unlockIdx < QixGameData.clearCharaters.Length &&
                        QixGameData.clearCharaters[unlockIdx]);

            case GalleryGameTyp.BlockOut:
                return (BlockOutGameData.clearedCharacters != null &&
                        unlockIdx >= 0 && unlockIdx < BlockOutGameData.clearedCharacters.Length &&
                        BlockOutGameData.clearedCharacters[unlockIdx]);

            case GalleryGameTyp.PingPong:
                return (PingPongGameDate.clearCharaters != null &&
                        unlockIdx >= 0 && unlockIdx < PingPongGameDate.clearCharaters.Length &&
                        PingPongGameDate.clearCharaters[unlockIdx]);

            case GalleryGameTyp.MatchGame:
                return (MatchGameGameData.clearCharater != null &&
                        unlockIdx >= 0 && unlockIdx < MatchGameGameData.clearCharater.Length &&
                        MatchGameGameData.clearCharater[unlockIdx]);

            default:
                return false;
        }
    }

    void UpdateGallery()
    {
        if (galleryList == null || galleryList.Length == 0) return;

        currentCharater = Mathf.Clamp(currentCharater, 0, galleryList.Length - 1);

        int unlockIdx = GetUnlockIndex();
        bool unlocked = IsUnlockedByIndex(unlockIdx);

        Debug.Log($"[UpdateGallery] charIdx={currentCharater}, unlockIdx={unlockIdx}, unlocked={unlocked}, name={galleryList[currentCharater].CharaterName}");

        if (!unlocked)
        {
            if (charaterNameText != null) charaterNameText.text = "Locked";
            if (displayImage != null) displayImage.sprite = lockedSprite;
            SetImageButtonsInteractable(false);
            return;
        }

        var data = galleryList[currentCharater];
        if (charaterNameText != null) charaterNameText.text = data.CharaterName;

        if (data.images == null || data.images.Length == 0)
        {
            if (displayImage != null) displayImage.sprite = null;
            SetImageButtonsInteractable(false);
            return;
        }

        bool perfectUnlocked = IsPerfectUnlockedByIndex(unlockIdx);


        if (data.images.Length >= 2 && !perfectUnlocked && currentImage >= 1)
            currentImage = 0;

        currentImage = Mathf.Clamp(currentImage, 0, data.images.Length - 1);

        if (displayImage != null)
            displayImage.sprite = data.images[currentImage];


        SetImageButtonsInteractable(data.images.Length > 1 && perfectUnlocked);
    }
    void SetImageButtonsInteractable(bool on)
    {
        if (imageLeftBtn != null) imageLeftBtn.interactable = on;
        if (imageRightBtn != null) imageRightBtn.interactable = on;
    }

    public void NextImage()
    {
        if (galleryList == null || galleryList.Length == 0) return;

        int unlockIdx = GetUnlockIndex();
        if (!IsUnlockedByIndex(unlockIdx)) return;

        var data = galleryList[currentCharater];
        if (data.images == null || data.images.Length == 0) return;

        currentImage = (currentImage + 1) % data.images.Length;
        UpdateGallery();
    }

    public void PreviousImage()
    {
        if (galleryList == null || galleryList.Length == 0) return;

        int unlockIdx = GetUnlockIndex();
        if (!IsUnlockedByIndex(unlockIdx)) return;

        var data = galleryList[currentCharater];
        if (data.images == null || data.images.Length == 0) return;

        currentImage--;
        if (currentImage < 0) currentImage = data.images.Length - 1;

        UpdateGallery();
    }

    public void NextCharater()
    {
        currentCharater++;
        if (currentCharater >= galleryList.Length) currentCharater = 0;

        currentImage = 0;
        UpdateGallery();
    }

    public void PreniousCharater()
    {
        currentCharater--;
        if (currentCharater < 0) currentCharater = galleryList.Length - 1;

        currentImage = 0;
        UpdateGallery();
    }

    void SetSpriteAnimated(Sprite s)
    {
        if (displayImage = null)
            return;
        if (_lastSprite == s)
            return;

        _lastSprite = s;
        if (displayGroup == null)
        {
            displayGroup = displayImage.GetComponent<CanvasGroup>();
            if (displayGroup == null)
                displayGroup = displayImage.gameObject.AddComponent<CanvasGroup>();
        }
        if (fadeCo != null)
            StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeSwap(s));
    }

    IEnumerator FadeSwap(Sprite next)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            displayGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        displayGroup.alpha = 1f;
    }

    bool IsPerfectUnlockedByIndex(int unlockIdx)
    {
        if (unlockIdx < 0)
            return false;

        switch (galleryGameTyp)
        {
            case GalleryGameTyp.Qix:
                return QixGameData.perfectClearCharacters != null &&
                       unlockIdx < QixGameData.perfectClearCharacters.Length &&
                       QixGameData.perfectClearCharacters[unlockIdx];

            case GalleryGameTyp.BlockOut:
                return BlockOutGameData.perfectClearCharacters != null &&
                       unlockIdx < BlockOutGameData.perfectClearCharacters.Length &&
                       BlockOutGameData.perfectClearCharacters[unlockIdx];

            case GalleryGameTyp.PingPong:
                return PingPongGameDate.perfectClearCharacters != null &&
                       unlockIdx < PingPongGameDate.perfectClearCharacters.Length &&
                       PingPongGameDate.perfectClearCharacters[unlockIdx];

            case GalleryGameTyp.MatchGame:
                return MatchGameGameData.perfectClearCharacters != null &&
                       unlockIdx < MatchGameGameData.perfectClearCharacters.Length &&
                       MatchGameGameData.perfectClearCharacters[unlockIdx];

            default:
                return false;
        }
    }
}
