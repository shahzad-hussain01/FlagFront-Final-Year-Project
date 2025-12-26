using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "Floating Text Settings", menuName = "Lovatto/Floating Text/Settings")]
public class FloatingTextSettings : ScriptableObject
{
    public FloatingType floatingDirectionType = FloatingType.FixedDirection;
    public Vector2 FloatDirection = new Vector2(1, 0);
    public AnimationCurve XFloatCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
    public AnimationCurve YFloatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector2 XFloatRange = new Vector2(-1, 1);
    public Vector2 YFloatRange = new Vector2(1, 1);

    public float StartSequenceDuration = 0.5f;
    public float StaticDuration = 0.5f;
    public float FloatingDuration = 1;
    public float FinishSequenceDuration = 0.2f;

    public AnimationCurve StartScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve FinishScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public AnimationCurve FadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve FadeOverLifeTime = AnimationCurve.EaseInOut(0, 1, 1, 1);
    public AnimationCurve FadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [System.Serializable]
    public enum FloatingType
    {
        FixedDirection,
        CurveAxis,
        RandomizeAxis,
    }
}