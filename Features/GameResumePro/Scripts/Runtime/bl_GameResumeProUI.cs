using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MFPS.Runtime.AI;

namespace MFPS.Addon.GameResumePro
{
    public class bl_GameResumeProUI : MonoBehaviour
    {
        public TextMeshProUGUI matchInfoText;
        public TextMeshProUGUI nextRoundCountText;
        public TextMeshProUGUI coinsText;
        public TextMeshProUGUI kdrText;
        public TextMeshProUGUI adoptedPlayerText;
        public TextMeshProUGUI accuracyText, headShotPercentageText;
        public TextMeshProUGUI xpText;
        public Image bestWeaponIcon;

        public GameObject levelProgressUI;
        public GameObject statsUI;
        public GameObject statPrefab;
        public RectTransform statPanel;
        public bl_GameResumeProProgress resumeProProgress;
        public GameObject[] windows;

        private int returnLobby = 10;
        private bl_GameResumePro resumeFetcher;
        private List<GameObject> cachedStats = new List<GameObject>();

        /// <summary>
        /// This is called after the final countdown has finished
        /// </summary>
        public void Show()
        {
            ChangeWindow(0);
            levelProgressUI.SetActive(false);
            gameObject.SetActive(true);
#if LM
            levelProgressUI.SetActive(true);
            statsUI.SetActive(false);
            resumeProProgress.Show(() =>
            {
                statsUI.SetActive(true);
            });
#endif
            InvokeRepeating(nameof(AutoLoadLobby), 0, 1);
        }

        /// <summary>
        /// This is called right after finish a round/game
        /// The final countdown has not started at this point
        /// </summary>
        /// <param name="fetcher"></param>
        public void FetchData(bl_GameResumePro fetcher)
        {
            resumeFetcher = fetcher;
            returnLobby = fetcher.returnToLobbyIn;
            var modeInfo = bl_RoomSettings.Instance.CurrentRoomInfo;
            matchInfoText.text = $"{modeInfo.gameMode.GetGameModeInfo().ModeName} | {modeInfo.mapName}";

            CalculateStats();
            CalculateBestWeapon();
            CalculatedAdoption();

            statPrefab.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateStats()
        {
            var player = bl_PhotonNetwork.LocalPlayer;
            int kills = player.GetKills();

            // # BEGGIN FOR MFPS 1.9.2 OR LATER
            // if bots eliminations doesn't count for the player stats
            if (bl_GameData.Instance.howConsiderBotsEliminations != BotKillConsideration.SameAsRealPlayers)
            {
                if (bl_RoomSettings.TryGetMatchPersistData("bot-kills", out var value))
                {
                    kills = Mathf.Min(0, kills - Mathf.FloorToInt((float)value));
                }
            }
            // # END FOR MFPS 1.9.2 OR LATER

            int deaths = player.GetDeaths();
            int score = player.GetPlayerScore();
            float kd = kills;
            if (kills <= 0) { kd = -deaths; }
            else if (deaths > 0) { kd = (float)kills / (float)deaths; }

            int secondsPlayed = resumeFetcher.GetStat("play-time");
            int minutesPlayed = secondsPlayed / 60;
            int scorePerMinute = minutesPlayed > 0 ? score / minutesPlayed : score;
            int shotsHits = resumeFetcher.GetStat("hits");
            int headShots = resumeFetcher.GetStat("hs");

            int winScore = bl_GameManager.Instance.isLocalPlayerWinner() ? bl_GameData.Instance.ScoreReward.ScoreForWinMatch : 0;
            int scorePerPlayedTime = bl_GameData.Instance.ScoreReward.GetScorePerTimePlayed(secondsPlayed);
            int totalScore = score + winScore + scorePerPlayedTime;
            resumeFetcher.SetStat("total-score-gained", totalScore);

            float hsPercentage = 0;
            if(shotsHits > 0 && headShots > 0)
            {
                hsPercentage = ((float)headShots / (float)shotsHits) * 100;
            }

            int totalCoinsGained = bl_GameData.Instance.VirtualCoins.GetCoinsPerScore(totalScore);
            resumeFetcher.SetStat("total-coins-gained", totalCoinsGained);

            cachedStats.ForEach(x => { Destroy(x.gameObject); });
            cachedStats.Clear();

            InstanceStat("Kills", kills);
            InstanceStat("Deaths", deaths);
            InstanceStat("Score", score);
            InstanceStat("Shots Fired", resumeFetcher.GetStat("bf"));
            InstanceStat("Shots Hit", shotsHits);
            InstanceStat("Head Shots", headShots);
            InstanceStat("Time Played", secondsPlayed);
            InstanceStat("Score Per Minute", scorePerMinute);
            InstanceStat("Total XP Gained", totalScore);
#if LM
            InstanceStat("Total Score", bl_LevelManager.Instance.GetSavedScore() + totalScore);
#endif

            kdrText.text = kd.ToString("0.0");
            coinsText.text = $"+{totalCoinsGained}";
            float accuracity = 0;
            if (shotsHits > 0) accuracity = ((float)shotsHits / (float)resumeFetcher.GetStat("bf")) * 100;
            accuracyText.text = $"{accuracity.ToString("0.0")}%";
            xpText.text = $"+{totalScore}";
            headShotPercentageText.text = $"{hsPercentage.ToString("0.0")}%";

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowID"></param>
        public void ChangeWindow(int windowID)
        {
            for (int i = 0; i < windows.Length; i++)
            {
                windows[i].SetActive(false);
            }
            windows[windowID].SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateBestWeapon()
        {
            int kills = 0;
            string weapon = "";
            for (int i = 0; i < resumeFetcher.stats.Count; i++)
            {
                if (!resumeFetcher.stats[i].Key.StartsWith("kw-")) continue;
                if (resumeFetcher.stats[i].Value <= kills) continue;

                weapon = resumeFetcher.stats[i].Key.Replace("kw-", "");
                kills = resumeFetcher.stats[i].Value;
            }

            if (string.IsNullOrEmpty(weapon))
            {

                return;
            }
            var weaponInfo = bl_GameData.Instance.GetWeaponID(weapon);
            bestWeaponIcon.sprite = bl_GameData.Instance.GetWeapon(weaponInfo).GunIcon;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculatedAdoption()
        {
            int kills = 0;
            string player = "";
            for (int i = 0; i < resumeFetcher.stats.Count; i++)
            {
                if (!resumeFetcher.stats[i].Key.StartsWith("kp-")) continue;
                if (resumeFetcher.stats[i].Value <= kills) continue;

                player = resumeFetcher.stats[i].Key.Replace("kp-", "");
                kills = resumeFetcher.stats[i].Value;
            }
            adoptedPlayerText.text = player;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InstanceStat(string statName, int value, string tag = "")
        {
            var obj = Instantiate(statPrefab) as GameObject;
            obj.SetActive(true);
            obj.transform.SetParent(statPanel, false);
            obj.GetComponent<bl_GameResumeProStat>().SetStat(statName, value.ToString(), tag);
            cachedStats.Add(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        void AutoLoadLobby()
        {
            returnLobby--;
            nextRoundCountText.text = $"RETURN TO LOBBY IN {returnLobby}";
            if (returnLobby > 0) return;
        
            //load lobby or new round
            LoadLobby();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadLobby()
        {
            CancelInvoke();
            if (bl_PhotonNetwork.IsConnected)
            {
                Photon.Pun.PhotonNetwork.LeaveRoom();
            }
            else
            {
                bl_UtilityHelper.LoadLevel(bl_GameData.Instance.MainMenuScene);
            }
        }
    }
}