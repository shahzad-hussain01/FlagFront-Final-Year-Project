using UnityEngine;
using System;
using MFPSEditor;

namespace MFPS.Addon.KillStreak
{
    [Serializable]
    public class KillStreakInfo
    {
        public string KillName = "Kill 0";
        public int ExtraScore = 0;
        [LovattoToogle] public bool Skip = false;
        [Header("References")]
        [SpritePreview(50)] public Sprite KillIcon;
        public AudioClip KillClip;

        [HideInInspector] public int killID = 0;
        [HideInInspector] public KillInfo info = null;
    }

    [Serializable]
    public class SpecialKillNotifier
    {
        public string key;
        public KillStreakInfo info;
    }

    [Serializable]
    public enum KillNotifierTextType
    {
        KillName,
        KillCount,
    }

    
}