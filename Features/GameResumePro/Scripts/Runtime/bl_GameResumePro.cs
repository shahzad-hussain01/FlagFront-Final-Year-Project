using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFPS.Internal.Interfaces;

namespace MFPS.Addon.GameResumePro
{
    public class bl_GameResumePro : MonoBehaviour, IMFPSResumeScreen
    {
        public int returnToLobbyIn = 60;
        public bl_GameResumeProUI resumeUI;

        public List<StatPropertie> stats = new List<StatPropertie>();

        private bool firstSpawn = false;
        private int killStreak, highestStreak = 0;

        /// <summary>
        /// 
        /// </summary>
        void OnEnable()
        {
            bl_UIReferences.Instance.ResumeScreen = this;
            bl_EventHandler.onLocalKill += OnLocalKill;
            bl_EventHandler.onLocalPlayerDeath += OnLocalPlayerDie;
            bl_EventHandler.onLocalPlayerFire += OnLocalPlayerFire;
            bl_EventHandler.onLocalPlayerSpawn += OnLocalPlayerSpawn;
            bl_EventHandler.onLocalPlayerHitEnemy += OnLocalPlayerHitAnEnemy;
            resumeUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnDisable()
        {
            bl_EventHandler.onLocalKill -= OnLocalKill;
            bl_EventHandler.onLocalPlayerDeath -= OnLocalPlayerDie;
            bl_EventHandler.onLocalPlayerFire -= OnLocalPlayerFire;
            bl_EventHandler.onLocalPlayerSpawn -= OnLocalPlayerSpawn;
            bl_EventHandler.onLocalPlayerHitEnemy -= OnLocalPlayerHitAnEnemy;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetStat(string key, float value) => SetStat(key, (int)value);

        /// <summary>
        /// 
        /// </summary>
        public void SetStat(string key, int value)
        {
            int index = stats.FindIndex(x => x.Key == key);
            if (index == -1)
            {
                stats.Add(new StatPropertie()
                {
                    Key = key,
                    Value = value
                });
                return;
            }
            stats[index].Value += value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetStat(string key)
        {
            int index = stats.FindIndex(x => x.Key == key);
            if (index == -1) return 0;

            return stats[index].Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CollectData()
        {
            var playTime = (int)Time.time - GetStat("start-time");
            SetStat("play-time", playTime);

            resumeUI.FetchData(this);
            SavePlayerData();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Show()
        {
            resumeUI.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SavePlayerData()
        {

            int totalScore = GetStat("total-score-gained");
            int totalCoins = GetStat("total-coins-gained");

#if ULSP
            if (bl_DataBase.Instance != null)
            {
                bl_ULoginMFPS.SaveLocalPlayerKDS(null, totalScore);
                bl_DataBase.Instance.StopAndSaveTime();
                if (totalCoins > 0)
                {
                    bl_DataBase.Instance.SaveNewCoins(totalCoins, bl_GameData.Instance.VirtualCoins.XPCoin);
                }
                
#if CLANS
                bl_DataBase.Instance.SetClanScore(totalScore);
#endif
            }
#else
            if (totalCoins > 0)
            {
                bl_GameData.Instance.VirtualCoins.AddCoins(totalCoins, bl_PhotonNetwork.LocalPlayer.NickName);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalKill(KillInfo killInfo)
        {
            if (killInfo.Killer != bl_PhotonNetwork.LocalPlayer.NickName) return;

            SetStat($"kw-{killInfo.KillMethod}", 1);
            SetStat($"kp-{killInfo.Killed}", 1);
            if(killInfo.byHeadShot) SetStat("hs", 1);
            killStreak++;          
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalPlayerDie()
        {
            SetStat("deaths", 1);
            highestStreak = Mathf.Max(killStreak, highestStreak);
            killStreak = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalPlayerFire(int gunID)
        {
            SetStat("bf", 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enemyName"></param>
        void OnLocalPlayerHitAnEnemy(MFPSHitData hitData)
        {
            SetStat("hits", 1);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalPlayerSpawn()
        {
            if (firstSpawn) return;

            SetStat("start-time", Time.time);
#if LM
            SetStat("start-score", bl_LevelManager.Instance.GetRuntimeLocalScore());
#endif
            firstSpawn = true;
        }

        public class StatPropertie
        {
            public string Key;
            public int Value;
        }
    }
}