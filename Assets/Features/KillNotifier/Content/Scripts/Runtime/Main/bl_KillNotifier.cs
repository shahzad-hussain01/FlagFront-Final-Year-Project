using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MFPS.Addon.KillStreak
{
    [RequireComponent(typeof(AudioSource))]
    public class bl_KillNotifier : MonoBehaviour
    {
        public GameObject Content;
        public Image KillLogo;
        public Text KillText;
        public CanvasGroup GlobalAlpha;
        public AnimationClip HideAnimation;
        public GameObject[] secundarysNotifiers;

        private bool isShowing = false;
        private KillStreakInfo currentInfo;
        private AudioSource ASource;

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            Content.SetActive(false);
            ASource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Show()
        {
            if (isShowing && !bl_KillNotifierData.Instance.OverrideOnNewStreak) return;

            isShowing = true;
            currentInfo = bl_KillStreakManager.Instance.GetQueueNotifier();
            if (currentInfo == null) return;

            ASource.Stop();
            if (currentInfo.info.byHeadShot && bl_KillNotifierData.Instance.prioretizeHeadShotNotification)
            {
                if (bl_KillNotifierData.TryGetNotification("headshot", out var info))
                {
                    SetupNotification(info);
                }
                else
                {
                    SetupNotification(currentInfo);
                }
            }
            else
            {
                SetupNotification(currentInfo);
            }

            ASource.volume = bl_KillNotifierData.Instance.volumeMultiplier;
            ASource.Play();

            StopAllCoroutines();
            StartCoroutine(DoDisplay());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void SetupNotification(KillStreakInfo info)
        {
            KillLogo.sprite = info.KillIcon;
            ASource.clip = info.KillClip;

            if (bl_KillNotifierData.Instance.killNotifierTextType == KillNotifierTextType.KillCount)
            {
                KillText.text = string.Format(bl_KillNotifierData.Instance.killCountFormat, bl_KillNotifierUtils.AddOrdinal(info.killID)).ToUpper();
            }
            else
            {
                KillText.text = info.KillName.ToUpper();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Hide()
        {
            StopAllCoroutines();
            GlobalAlpha.alpha = 0;
            Content.SetActive(false);
            foreach (GameObject g in secundarysNotifiers) { g.SetActive(false); }
            isShowing = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator DoDisplay()
        {
            GlobalAlpha.alpha = 1;
            Content.SetActive(false);
            foreach (GameObject g in secundarysNotifiers)
            {
                if (g == null) continue;
                
                g.SetActive(false);
            }
            
            Content.SetActive(true);
            if (currentInfo.info.byHeadShot && !bl_KillNotifierData.Instance.prioretizeHeadShotNotification)
            {
                secundarysNotifiers[0].SetActive(true);
            }

            yield return new WaitForSeconds(bl_KillNotifierData.Instance.TimeToShow);
            if (HideAnimation == null)
            {
                float d = 1;
                while (d > 0)
                {
                    d -= Time.deltaTime * 3;
                    GlobalAlpha.alpha = d;
                    yield return null;
                }
            }
            else
            {
                Content.GetComponent<Animator>().Play("hide", 0, 0);
                yield return new WaitForSeconds(HideAnimation.length);
            }
            isShowing = false;
            Show();
        }

        private static bl_KillNotifier _instance;
        public static bl_KillNotifier Instance
        {
            get
            {
                if (_instance == null) { _instance = FindObjectOfType<bl_KillNotifier>(); }
                return _instance;
            }
        }
    }
}