using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatchGameStageDB
{
    public static readonly Dictionary<MatchGameCharacter,string[]>StageScenes=new Dictionary<MatchGameCharacter, string[]>

    {
      {MatchGameCharacter.IchinoseAsuna,new string[]
      {
        "03.Imgs/07.MatchGame/01.IchinoseAsuna/IchinoseAsuna01",
        "03.Imgs/07.MatchGame/01.IchinoseAsuna/IchinoseAsuna02",
        "03.Imgs/07.MatchGame/01.IchinoseAsuna/IchinoseAsuna03"
            
      }
      },
      {MatchGameCharacter.KosakaWakamo,new string[]
      {
        "03.Imgs/07.MatchGame/02.KosakaWakamo/KosakaWakamo01",
        "03.Imgs/07.MatchGame/02.KosakaWakamo/KosakaWakamo02",
        "03.Imgs/07.MatchGame/02.KosakaWakamo/KosakaWakamo03" 
      }
      },
      {MatchGameCharacter.MisonoMika,new string[]
      {
        "03.Imgs/07.MatchGame/03.MisonoMika/MisonoMika01",
        "03.Imgs/07.MatchGame/03.MisonoMika/MisonoMika02",
        "03.Imgs/07.MatchGame/03.MisonoMika/MisonoMika03"
      }
      },
      {MatchGameCharacter.AsagiMutsuki,new string[]
      {
        "03.Imgs/07.MatchGame/04.AsagiMutsuki/AsagiMutsuki01",
        "03.Imgs/07.MatchGame/04.AsagiMutsuki/AsagiMutsuki02",
        "03.Imgs/07.MatchGame/04.AsagiMutsuki/AsagiMutsuki03"
      }}
    };

    public static readonly Dictionary<MatchGameCharacter,string>ClearScenes=new Dictionary<MatchGameCharacter, string>
    {
        {MatchGameCharacter.IchinoseAsuna,"03.IchinoseAsunaClear"},
        {MatchGameCharacter.KosakaWakamo,"04.KosakaWakamoClear"},
        {MatchGameCharacter.MisonoMika,"05.MisonoMikaClear"},
        {MatchGameCharacter.AsagiMutsuki,"06.AsagiMutsukiClear"},
    };
}
