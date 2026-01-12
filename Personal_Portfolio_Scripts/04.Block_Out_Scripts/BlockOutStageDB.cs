using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockOutStageDB
{
    public static readonly Dictionary<BlockOutCharacterType, int> MaxStage =
    new Dictionary<BlockOutCharacterType, int>
    {
        {BlockOutCharacterType.HuTao,3},
        {BlockOutCharacterType.Furina,3},
        {BlockOutCharacterType.Nilou,3},
        {BlockOutCharacterType.Yoimiya,3}

    };
    public static readonly Dictionary<BlockOutCharacterType, string> ShowScenes =
    new Dictionary<BlockOutCharacterType, string>
    {
        {BlockOutCharacterType.HuTao,"03.HutaoShow"},
        {BlockOutCharacterType.Furina,"04.FurinaShow"},
        {BlockOutCharacterType.Nilou,"05.NilouShow"},
        {BlockOutCharacterType.Yoimiya,"06.YoimiyaShow"}
    };

    public static readonly Dictionary<BlockOutCharacterType,string>ClearScenes=
    new Dictionary<BlockOutCharacterType, string>
    {
        {BlockOutCharacterType.HuTao,"07.HutaoClear"},
        {BlockOutCharacterType.Furina,"08.FurinaClear"},
        {BlockOutCharacterType.Nilou,"09.NilouClear"},
        {BlockOutCharacterType.Yoimiya,"10.YoimiyaClear"}
    };
}
