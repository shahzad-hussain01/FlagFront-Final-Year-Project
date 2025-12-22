using UnityEngine;
using System.Collections.Generic;
using MFPS.Addon.KillStreak;

[RequireComponent(typeof(AudioSource))]
public class bl_KillStreakManager : MonoBehaviour
{
    public int currentStreak { get; set; }
    public Queue<KillStreakInfo> queueNotifiers = new Queue<KillStreakInfo>();

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        bl_EventHandler.onLocalKill += OnLocalKill;
        bl_EventHandler.onLocalPlayerDeath += OnLocalPlayerDeath;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        bl_EventHandler.onLocalKill -= OnLocalKill;
        bl_EventHandler.onLocalPlayerDeath -= OnLocalPlayerDeath;
    }

    /// <summary>
    /// 
    /// </summary>
    void OnLocalKill(KillInfo info)
    {
        currentStreak++;
        KillStreakInfo notifierInfo = bl_KillNotifierData.Instance.GetKillStreakInfo(currentStreak);
        if (notifierInfo.Skip) return;

        notifierInfo.killID = currentStreak;
        notifierInfo.info = info;

        if (notifierInfo.ExtraScore > 0)
        {
            bool isBot = IsBot(info.Killed);
            if (isBot && bl_GameData.Instance.howConsiderBotsEliminations == MFPS.Runtime.AI.BotKillConsideration.SameAsRealPlayers)
            {
                bl_PhotonNetwork.LocalPlayer.PostScore(notifierInfo.ExtraScore);
            }
            else if (!isBot)
            {
                bl_PhotonNetwork.LocalPlayer.PostScore(notifierInfo.ExtraScore);
            }
        }

        queueNotifiers.Enqueue(notifierInfo);
        if (queueNotifiers.Count <= 1)
        {
            //start showing streaks UI
            bl_KillNotifier.Instance.Show();
        }
#if KSA
        bl_KillStreakHandler.Instance.OnNewKill(currentStreak);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerName"></param>
    /// <returns></returns>
    private bool IsBot(string playerName)
    {
        if (bl_AIMananger.Instance.GetBotStatistics(playerName) != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public KillStreakInfo GetQueueNotifier()
    {
        if (queueNotifiers.Count > 0)
        {
            return queueNotifiers.Dequeue();
        }
        else { return null; }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnLocalPlayerDeath()
    {
        ResetStreak();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetStreak()
    {
        currentStreak = 0;
        bl_KillNotifier.Instance.Hide();
    }

    private static bl_KillStreakManager _instance;
    public static bl_KillStreakManager Instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType<bl_KillStreakManager>(); }
            return _instance;
        }
    }
}