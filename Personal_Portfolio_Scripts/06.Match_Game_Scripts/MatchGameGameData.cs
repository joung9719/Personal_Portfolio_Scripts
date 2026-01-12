using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatchGameGameData
{
    public static MatchGameCharacter SelectedCharacter = MatchGameCharacter.IchinoseAsuna;
    public static int CurrentStage = 0;

    public static int life = 3;
    public static int score = 0;
    public static int round;
    public static float remainTime = 900f;

    public static bool[] clearCharater = new bool[4];
    public static bool isPerfectRun = true;
    public static bool[] perfectClearCharacters = new bool[4];

}
