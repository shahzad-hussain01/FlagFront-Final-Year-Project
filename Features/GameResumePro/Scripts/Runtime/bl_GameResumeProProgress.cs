using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MFPS.Audio;
using TMPro;

namespace MFPS.Addon.GameResumePro
{
    public class bl_GameResumeProProgress : MonoBehaviour
    {
        [Header("Settings")]
        public float fillBarDuration = 2;
        public float newLevelFadeDuration = 0.3f;

        public AnimationCurve barCompleteAlpha;

        [Header("References")]
        public bl_AudioBank audioBank;
        public Image progressBarBase;
        public Slider progressBarDifference;
        public TextMeshProUGUI gainedXpText;
        public TextMeshProUGUI relativeXpText;
        public TextMeshProUGUI levelNameText;
        public bl_GameResumePro resumeFetcher;
        public CanvasGroup barAlpha;
        public bl_GRLevelBox[] levelBoxes;

        private AudioSource audioSource;
        private int animationState = 0;
        private Action animationCallback;

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            if(animationState == -1)
            {
                SkipToFinish();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            if (animationState == 1) animationState = -1;//animation canceled before finish
        }

        /// <summary>
        /// 
        /// </summary>
        public void Show(Action callback)
        {
            animationState = 1;
            animationCallback = callback;
            SetupInit();
            StartCoroutine(PlayProgressSequence(callback));
        }

        /// <summary>
        /// 
        /// </summary>
        void SetupInit()
        {
            audioSource = GetComponent<AudioSource>();
            progressBarBase.fillAmount = 0;
#if LM
            var level = bl_LevelManager.Instance.GetLevel(resumeFetcher.GetStat("start-score"));
            levelBoxes[0].Set(level);
            levelBoxes[1].Set(bl_LevelManager.Instance.GetLevelByID(level.LevelID));
            relativeXpText.text = $"{resumeFetcher.GetStat("start-score")} / {level.ScoreNeeded}";
            levelNameText.text = level.Name.ToUpper();
#endif  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayProgressSequence(Action callback)
        {
#if LM
            int startScore = resumeFetcher.GetStat("start-score");
            int newScore = startScore + resumeFetcher.GetStat("total-score-gained");

            //the player level with which start the game
            int initialLevel = bl_LevelManager.Instance.GetLevelID(startScore);
            //the player level after the game finish and apply the new score/xp
            int newLevel = bl_LevelManager.Instance.GetLevelID(newScore);
            //the levels difference
            int levelsChanged = (newLevel - initialLevel) + 1;
            gainedXpText.text = $"{startScore} XP";

            //set the initial level percentage complete
            float percentage = progressBarDifference.value = bl_LevelManager.Instance.GetRelaviteScorePercentage(startScore);
            progressBarBase.fillAmount = percentage;

            yield return new WaitForSeconds(2);

            int awardScore = 0;
            for (int i = 0; i < levelsChanged; i++)
            {
                int currentLevelID = initialLevel + i;
                var currentLevelInfo = bl_LevelManager.Instance.GetLevelByID(currentLevelID);
                var nextLevelInfo = bl_LevelManager.Instance.GetLevelByID(currentLevelID + 1);

                float currentPercentage = 1;
                //the xp that the player will earn for this level
                int toGainXp = nextLevelInfo.GetRelativeScoreNeeded();
                levelNameText.text = currentLevelInfo.Name.ToUpper();

                if (i == 0)
                {
                    //deduct the xp that already gain
                    toGainXp -= Mathf.FloorToInt(toGainXp * percentage);
                }
                else
                {
                    progressBarBase.fillAmount = 0;
                    progressBarDifference.value = 0.002f;
                    percentage = 0;
                    if (i == levelsChanged - 1)//if this is the last new level
                    {
                        //calculate the percentage based in the score (instead of 1)
                        currentPercentage = bl_LevelManager.Instance.GetRelaviteScorePercentage(newScore);
                    }
                    //deduct the remain level score needed
                    toGainXp -= Mathf.FloorToInt(toGainXp * (1 - currentPercentage));

                    audioBank.PlayAudioInSource(audioSource, "new-level");
                    levelBoxes[0].Set(bl_LevelManager.Instance.GetLevelByID(currentLevelID));
                    levelBoxes[1].Set(bl_LevelManager.Instance.GetLevelByID(currentLevelID + 1));

                    yield return new WaitForSeconds(0.7f);
                }

                float d = 0;
                float p = percentage;
                float duration = fillBarDuration;
                if (currentPercentage > 0) duration *= currentPercentage;

                var ainfo = audioBank.PlayAudioInSource(audioSource, "bar");
                audioSource.pitch = 0.1f + Mathf.Max(0.75f, ainfo.Clip.length / duration);

                int relativeNeededScore = currentLevelInfo.GetRelativeScoreNeeded();
                levelNameText.text = currentLevelInfo.Name.ToUpper();

                while (d < 1)
                {
                    d += Time.deltaTime / duration;
                    p = Mathf.Lerp(percentage, currentPercentage, d);

                    int xp = Mathf.FloorToInt(toGainXp * d);
                    progressBarDifference.value = p;
                    gainedXpText.text = $"AWARD SCORE + {awardScore + xp} XP";
                    relativeXpText.text = $"{xp} / {relativeNeededScore}";
                    yield return null;
                }
                awardScore += toGainXp;
                gainedXpText.text = $"AWARD SCORE + {awardScore} XP";
                relativeXpText.text = $"{Mathf.FloorToInt(relativeNeededScore * currentPercentage)} / {relativeNeededScore}";

                audioSource.Stop();
                yield return StartCoroutine(NewLevelSequence());
            }
            gainedXpText.text = $"AWARD SCORE + {resumeFetcher.GetStat("total-score-gained")} XP";
            animationState = 2;
            callback?.Invoke();
#else
            callback?.Invoke();
            yield break;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void SkipToFinish()
        {
#if LM
            var levelID = bl_LevelManager.Instance.GetRuntimeLevelID();
            var currentScore = bl_LevelManager.Instance.GetRuntimeLocalScore();
            var oldScore = resumeFetcher.GetStat("start-score");
            var oldLevelID = bl_LevelManager.Instance.GetLevel(oldScore);

            levelBoxes[0].Set(bl_LevelManager.Instance.GetLevelByID(levelID));
            levelBoxes[1].Set(bl_LevelManager.Instance.GetLevelByID(levelID + 1));
            barAlpha.alpha = 1;

            var currentPercentage = bl_LevelManager.Instance.GetRelaviteScorePercentage(currentScore);
            progressBarDifference.value = currentPercentage;
            var relativeNeeded = bl_LevelManager.Instance.GetLevelByID(levelID).GetRelativeScoreNeeded();
            relativeXpText.text = $"{Mathf.FloorToInt(relativeNeeded * currentPercentage)} / {relativeNeeded}";
            levelNameText.text = bl_LevelManager.Instance.GetLevelByID(levelID).Name.ToUpper();

            if (levelID == oldLevelID.LevelID)
            {
                currentPercentage = bl_LevelManager.Instance.GetRelaviteScorePercentage(oldScore);
                progressBarBase.fillAmount = currentPercentage;
            }
            else progressBarBase.fillAmount = 0;

            var awardScore = currentScore - oldScore;

            gainedXpText.text = $"AWARD SCORE + {awardScore} XP";
#endif
            animationCallback?.Invoke();
            animationState = 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator NewLevelSequence()
        {
            float d = 0;
            float t = 0;
            while(d < 1)
            {
                d += Time.deltaTime / newLevelFadeDuration;
                t = barCompleteAlpha.Evaluate(d);
                barAlpha.alpha = t;
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);

        }
    }
}