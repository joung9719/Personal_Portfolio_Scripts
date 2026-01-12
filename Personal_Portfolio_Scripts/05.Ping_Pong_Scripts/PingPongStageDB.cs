using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PingPongStageDB
{
    public static readonly Dictionary<PingPongCharacter,string[]>StageScenes=new Dictionary<PingPongCharacter, string[]>
    {
       {PingPongCharacter.EllenJoe,new string[]{"PP_EllenJoeStage_01","PP_EllenJoeStage_02","PP_EllenJoeStage_03"}},
       {PingPongCharacter.Miyabi,new string[]{"PP_MiyabiStage_01","PP_MiyabiStage_02","PP_MiyabiStage_03"}},
       {PingPongCharacter.Xyuan,new string[]{"PP_YixuanStage_01","PP_YixuanStage_02","PP_YixuanStage_03"}},
       {PingPongCharacter.JanDoe,new string[]{"PP_JaneDoeStage_01","PP_JaneDoeStage_02","PP_JaneDoeStage_03"}} 
               
    };

    public static readonly Dictionary<PingPongCharacter,string>ShowScene=new Dictionary<PingPongCharacter, string>
    {
        {PingPongCharacter.EllenJoe,"PP_EllenJoeShow_04"},
        {PingPongCharacter.Miyabi,"PP_MiyabiShow_04"},
        {PingPongCharacter.Xyuan,"PP_YixuanShow_04"},
        {PingPongCharacter.JanDoe,"PP_JaneDoeShow_04"},
    };

    public static readonly Dictionary<PingPongCharacter,string>ClearScenes=new Dictionary<PingPongCharacter, string>
    {
        {PingPongCharacter.EllenJoe,"PP_EllenJoeClear_05"},
        {PingPongCharacter.Miyabi,"PP_MiyabiClear_05"},
        {PingPongCharacter.Xyuan,"PP_XyuanClear_05"},
        {PingPongCharacter.JanDoe,"PP_JanDoeClear_05"},
    };
}
