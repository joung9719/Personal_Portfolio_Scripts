using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockOutGameData
{
    public static BlockOutCharacterType SelectedCharacter=BlockOutCharacterType.HuTao;
    public static int CurrentStage=1;
    public static bool[] clearedCharacters =new bool[4];
    public static bool isPerfectRun=true;
    public static bool[] perfectClearCharacters=new bool[4];
   
}
