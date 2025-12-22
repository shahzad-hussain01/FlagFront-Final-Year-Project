using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Addon.MatchMakingPro
{
    [Serializable]
    public class GameModeExtraInfo
    {
        public string GameMode;
        public GameMode Identifier;
        [LovattoToogle] public bool IsSoloMode = false;
        [TextArea(3, 5)] public string Description;
        public Texture2D BackgroundImage;
    }
}