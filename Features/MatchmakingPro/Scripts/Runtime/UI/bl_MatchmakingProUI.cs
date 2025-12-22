using MFPS.Internal;
using MFPS.Runtime.Settings;
using MFPS.Runtime.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace MFPS.Addon.MatchMakingPro
{
    public class bl_MatchmakingProUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] windows = null;
        [SerializeField] private UIListHandler gameModesList = null;
        [SerializeField] private TextMeshProUGUI descriptionText = null;
        [SerializeField] private TextMeshProUGUI timeLapseText = null;
        [SerializeField] private TextMeshProUGUI stateText = null;
        [SerializeField] private GameObject cancelMatchmakingBtn = null;
        public bl_ConfirmationWindow confirmationWindow = null;
        [SerializeField] private AudioClip gameModeSelectedSound = null;
        [SerializeField] private RectTransform rightSpaceRect = null;
        [SerializeField] private bl_SingleSettingsBinding roomTypeOption = null;

        private int matchmakingTime = 0;
        private bool initialized = false;
        private AudioSource audioSource;

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            TryGetComponent(out audioSource);          
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            descriptionText.text = string.Empty;
            SetActiveWindow(0);

            if (!initialized)
            {
                StartCoroutine(Delayed());
                IEnumerator Delayed()
                {
                    while (!bl_GameData.isDataCached)
                    {
                        yield return null;
                    }

                    gameModesList.Initialize();
                    InstanceGameModes();
                    initialized = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowMatchmaking()
        {
            SetActiveWindow(1);
            matchmakingTime = 0;
            InvokeRepeating(nameof(IncreaseTime), 0, 1);
            cancelMatchmakingBtn.SetActive(true);
            audioSource.clip = gameModeSelectedSound;
            audioSource.Play();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CancelMatchmaking()
        {
            CancelInvoke();
            SetActiveWindow(0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnMatchFound()
        {
            cancelMatchmakingBtn.SetActive(false);
            CancelInvoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void SetActiveWindow(int id)
        {
            foreach (var item in windows)
            {
                item.SetActive(false);
            }
            windows[id].SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InstanceGameModes()
        {
            var availableModes = bl_GameData.Instance.gameModes.Where(x => x.isEnabled).ToList();

            for (int i = 0; i < availableModes.Count; i++)
            {
                var script = gameModesList.InstatiateAndGet<bl_ModeCardOptionUI>();
                script.Init(availableModes[i]);
            }

            if (rightSpaceRect != null)
            {
                rightSpaceRect.SetAsLastSibling();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public static void SetStateText(string text)
        {
            if (Instance == null) return;

            Instance.stateText.text = text.ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameMode"></param>
        public void PreviewMode(GameModeExtraInfo gameMode)
        {
            if (gameMode == null) return;

            descriptionText.text = gameMode.Description.ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        void IncreaseTime()
        {
            matchmakingTime++;
            // convert the seconds to time format string
            timeLapseText.text = bl_StringUtility.GetTimeFormat(matchmakingTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRoomOptionPublic() => roomTypeOption.currentOption == 0;

        private static bl_MatchmakingProUI _instance = null;
        public static bl_MatchmakingProUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<bl_MatchmakingProUI>();
                }
                return _instance;
            }
        }
    }
}