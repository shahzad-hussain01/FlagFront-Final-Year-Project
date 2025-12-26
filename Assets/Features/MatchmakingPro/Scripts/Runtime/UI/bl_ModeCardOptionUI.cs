using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFPS.Addon.MatchMakingPro
{
    public class bl_ModeCardOptionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI modeNameText = null;
        [SerializeField] private RawImage backgroundImg = null;
        [SerializeField] private Image buttonImg = null;
        [SerializeField] private AudioClip onHoverSound = null;
        [SerializeField] private Image teamModeImg = null;
        [SerializeField] private Sprite[] teamModeSprites = null;

        private GameModeSettings cacheGameMode;
        private GameModeExtraInfo extraInfo;
        private AudioSource audioSource;

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            buttonImg.alphaHitTestMinimumThreshold = 0.3f;
            TryGetComponent(out audioSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init(GameModeSettings gameMode)
        {
            cacheGameMode = gameMode;
            modeNameText.text = gameMode.ModeName.ToUpper();

            int index = bl_MatchmakingProSettings.Instance.gameModes.FindIndex(x => x.Identifier == cacheGameMode.gameMode);
            if (index == -1)
            {
                Debug.LogWarning($"The game mode {cacheGameMode.ModeName} doesn't have the extra information to use in Matchmaking Pro.");
                return;
            }

            extraInfo = bl_MatchmakingProSettings.Instance.gameModes[index];
            backgroundImg.texture = extraInfo.BackgroundImage;
            teamModeImg.sprite = extraInfo.IsSoloMode ? teamModeSprites[0] : teamModeSprites[1];
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnClick()
        {
            bl_MatchmakingPro.Instance.FindMatchForGameMode(cacheGameMode);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnHover()
        {
            bl_MatchmakingProUI.Instance.PreviewMode(extraInfo);
            audioSource.clip = onHoverSound;
            audioSource.Play();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnClickRandom()
        {
            bl_MatchmakingPro.Instance.PlayRandomMode();
        }
    }
}