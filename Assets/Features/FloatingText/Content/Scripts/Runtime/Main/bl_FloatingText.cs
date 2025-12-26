using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.FloatingTextAsset
{
    public class bl_FloatingText : MonoBehaviour
    {      
        public UnityText uiText;
        public CanvasGroup canvasGroup;
        public int UseTimes { get; set; } = -1;

        #region Private members       
        private Vector3 rectPosition;
        private int defaultTextSize = -1;
        private Vector3 targetScreenPosition;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Set(FloatingText data)
        {
            if (uiText != null)
            {
                uiText.text = data.Text;
                uiText.color = data.TextColor;
                if (defaultTextSize == -1) defaultTextSize = (int)uiText.fontSize;
                uiText.fontSize = defaultTextSize + data.ExtraTextSize;

                if (data.OutlineColor != Color.clear) uiText.outlineColor = data.OutlineColor;
                if (data.OutlineSize != -1) uiText.outlineSize = data.OutlineSize;
            }

            var result = CheckForReuse(data);

            gameObject.SetActive(true);

            //if this is a Reuse and is marked as not rewind on reuses
            if (result == 2 && data.Flags.HasFlag(FloatingTextFlags.DontRewind)) return; //don't re-play the sequence

            //set the start position
            RectTransformRef.position = bl_FloatingTextManager.GetScreenPositionFromWorldPosition(data.GetPosition());
            data.InternalOnly.InitRectPosition = RectTransformRef.position;
            targetScreenPosition = RectTransformRef.position;

            if (data.Flags.HasFlag(FloatingTextFlags.InvertHorizontalDirectionRandomly))
                data.InternalOnly.InvertHorizontalDirection = Random.value > 0.5f;

            //get a random float direction
            if (data.Settings.floatingDirectionType == FloatingTextSettings.FloatingType.RandomizeAxis)
                data.InternalOnly.FloatDirection = new Vector2(Random.Range(data.Settings.XFloatRange.x, data.Settings.XFloatRange.y), Random.Range(data.Settings.YFloatRange.x, data.Settings.YFloatRange.y));

            StopAllCoroutines();
            StartCoroutine(DoSequence(data));
        }

        /// <summary>
        /// Check if we can reuse this text instance
        /// </summary>
        /// <param name="data"></param>
        private int CheckForReuse(FloatingText data)
        {
            if (data.Target == null || data.ReuseTimes == -1 || data.ReuseTimes == 0) return 0;

            if (UseTimes == -1)//if is the first time using this instance
            {
                bl_FloatingTextManager.Instance.AddToReused(this, data);
                UseTimes = data.ReuseTimes;
            }
            else if (UseTimes > 0)//if this is an re-use
            {
                UseTimes--;
                return 2;
            }
            else if (UseTimes == 0)//if this is the last allow re-use time
            {
                bl_FloatingTextManager.Instance.RemoveFromReused(data);
                return 2;
            }
            else if (UseTimes == -2) return 2;//we can reuse this instance until the sequence finish

            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IEnumerator DoSequence(FloatingText data)
        {
            var settings = data.Settings;

            float sequenceTime = settings.StartSequenceDuration + settings.FloatingDuration + settings.FinishSequenceDuration;
            float globalPercentage = 0;
            float percentageAcumulate = settings.StartSequenceDuration / sequenceTime;
            float tempAcu;
            float baseAlpha;
            rectPosition = Vector3.zero;

            //the start/appear sequence
            float d = 0;
            if (settings.StartSequenceDuration > 0)
                while (d < 1)
                {
                    d += Time.deltaTime / settings.StartSequenceDuration;
                    canvasGroup.alpha = settings.FadeInCurve.Evaluate(d);
                    RectTransformRef.localScale = Vector3.one * settings.StartScaleCurve.Evaluate(d);

                    if (settings.floatingDirectionType == FloatingTextSettings.FloatingType.CurveAxis)
                        globalPercentage = percentageAcumulate * d;

                    if (settings.StaticDuration <= 0) Move(data, globalPercentage);
                    yield return null;
                }

            if (settings.StaticDuration > 0)
            {
                if (data.Flags.HasFlag(FloatingTextFlags.StickAtOriginPosition))
                {
                    d = 0;
                    while (d < 1)
                    {
                        d += Time.deltaTime / settings.StaticDuration;
                        targetScreenPosition = bl_FloatingTextManager.GetScreenPositionFromWorldPosition(data.GetPosition());
                        RectTransformRef.position = targetScreenPosition;
                        yield return null;
                    }
                }
                else
                    yield return new WaitForSeconds(settings.StaticDuration);
            }

            //the floating sequence
            d = 0;
            tempAcu = (settings.FloatingDuration / sequenceTime);
            if (settings.StaticDuration > 0)
            {
                tempAcu += percentageAcumulate;
                percentageAcumulate = 0;
            }
            if (settings.FloatingDuration > 0)
                while (d < 1)
                {
                    d += Time.deltaTime / settings.FloatingDuration;

                    canvasGroup.alpha = settings.FadeOverLifeTime.Evaluate(d);
                    if (settings.floatingDirectionType == FloatingTextSettings.FloatingType.CurveAxis)
                        globalPercentage = percentageAcumulate + (tempAcu * d);

                    Move(data, globalPercentage);
                    yield return null;
                }
            percentageAcumulate += tempAcu;

            //finish/hide sequence
            d = 0;
            baseAlpha = canvasGroup.alpha;
            tempAcu = (settings.FinishSequenceDuration / sequenceTime);
            if (UseTimes == -2) bl_FloatingTextManager.Instance.RemoveFromReused(data);
            if (settings.FinishSequenceDuration > 0)
                while (d < 1)
                {
                    d += Time.deltaTime / settings.FinishSequenceDuration;
                    canvasGroup.alpha = baseAlpha * settings.FadeOutCurve.Evaluate(d);
                    RectTransformRef.localScale = Vector3.one * settings.FinishScaleCurve.Evaluate(d);

                    if (settings.floatingDirectionType == FloatingTextSettings.FloatingType.CurveAxis)
                        globalPercentage = percentageAcumulate + (tempAcu * d);

                    Move(data, globalPercentage);
                    yield return null;
                }

            data.FinishCallback?.Invoke();
            Disable();
        }

        /// <summary>
        /// 
        /// </summary>
        void Disable()
        {
            UseTimes = -1;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Apply the floating movement
        /// </summary>
        /// <param name="data"></param>
        private void Move(FloatingText data, float percentage)
        {
            if (data.Flags.HasFlag(FloatingTextFlags.StickAtOriginPosition))
            {
                if((Time.frameCount % 2) == 0)
                {
                    targetScreenPosition = bl_FloatingTextManager.GetScreenPositionFromWorldPosition(data.GetPosition());
                }
            }

            if (data.Settings.floatingDirectionType == FloatingTextSettings.FloatingType.FixedDirection)
            {
                rectPosition += (Vector3)data.Settings.FloatDirection;
            }
            else if (data.Settings.floatingDirectionType == FloatingTextSettings.FloatingType.CurveAxis)
            {
                rectPosition.x = data.Settings.XFloatCurve.Evaluate(percentage);
                rectPosition.y = data.Settings.YFloatCurve.Evaluate(percentage);
            }
            else if (data.Settings.floatingDirectionType == FloatingTextSettings.FloatingType.RandomizeAxis)
            {
                rectPosition += (Vector3)data.InternalOnly.FloatDirection;
            }

            if (data.InternalOnly.InvertHorizontalDirection) rectPosition.x = -rectPosition.x;

            RectTransformRef.position = targetScreenPosition + rectPosition;
        }

        /// <summary>
        /// Is this instance currently active?
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        private RectTransform m_rectTransform;
        public RectTransform RectTransformRef
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = (RectTransform)transform;
                return m_rectTransform;
            }
        }
    }
}