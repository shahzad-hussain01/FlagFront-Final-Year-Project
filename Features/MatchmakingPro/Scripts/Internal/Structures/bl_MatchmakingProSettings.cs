using System.Collections.Generic;
using UnityEngine;
using MFPS.Addon.MatchMakingPro;

[CreateAssetMenu( fileName = "MatchmakingProSettings", menuName = "MFPS/Addons/Settings/MatchmakingPro")]
public class bl_MatchmakingProSettings : ScriptableObject
{
    public enum CreatedRoomBehave
    {
        JoinDirectly,
        WaitForAnotherPlayer
    }

    public List<GameModeExtraInfo> gameModes;
    public CreatedRoomBehave onCreatedRoomBehave = CreatedRoomBehave.JoinDirectly;

    private static bl_MatchmakingProSettings m_Data;
    public static bl_MatchmakingProSettings Instance
    {
        get
        {
            if (m_Data == null)
            {
                m_Data = Resources.Load("MatchmakingProSettings", typeof(bl_MatchmakingProSettings)) as bl_MatchmakingProSettings;
            }
            return m_Data;
        }
    }
}