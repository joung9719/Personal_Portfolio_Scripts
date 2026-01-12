using System.Collections.Generic;
using UnityEngine;

public enum QixCharacterType
{
    Sora,
    D,
    Nayuta,
    Liberialo
}

[System.Serializable]
public class StageInfo
{
    public Sprite background;
    public float enemySpeed;
    public int enemyCount;

    public StageInfo(string path, float speed, int count)
    {
        background = Resources.Load<Sprite>(path);
        enemySpeed = speed;
        enemyCount = count;

        if (background == null)
        Debug.LogError($"[StageDB] 배경 로드 실패: {path}");
    }
}

public static class QixStageDB
{
    private const string BasePath = "03.Imgs/04.Qix";

    public static Dictionary<QixCharacterType, StageInfo[]> stages =
        new Dictionary<QixCharacterType, StageInfo[]>
    {
        {
            QixCharacterType.Sora,
            new StageInfo[]
            {
                new StageInfo($"{BasePath}/01.Sora/sora01", 5f, 2),
                new StageInfo($"{BasePath}/01.Sora/sora02", 7f, 4),
                new StageInfo($"{BasePath}/01.Sora/sora03", 9f, 5)
            }
        },
        {
            QixCharacterType.D,
            new StageInfo[]
            {
                new StageInfo($"{BasePath}/02.D/D01", 5f, 2),
                new StageInfo($"{BasePath}/02.D/D02", 7f, 4),
                new StageInfo($"{BasePath}/02.D/D03", 9f, 5)
            }
        },
        {
            QixCharacterType.Nayuta,
            new StageInfo[]
            {
                new StageInfo($"{BasePath}/03.Nayuta/Nayuta01", 5f, 2),
                new StageInfo($"{BasePath}/03.Nayuta/Nayuta02", 7f, 4),
                new StageInfo($"{BasePath}/03.Nayuta/Nayuta03", 9f, 5)
            }
        },
        {
            QixCharacterType.Liberialo,
            new StageInfo[]
            {
                new StageInfo($"{BasePath}/04.Liberialo/Liberialo01", 3f, 1),
                new StageInfo($"{BasePath}/04.Liberialo/Liberialo02", 4f, 3),
                new StageInfo($"{BasePath}/04.Liberialo/Liberialo03", 5f, 5)
            }
        },
    };
}
