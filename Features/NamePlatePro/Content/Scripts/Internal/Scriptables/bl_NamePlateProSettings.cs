using UnityEngine;

[CreateAssetMenu(fileName = "NamePlateProSettings", menuName = "MFPS/Settings/NamePlatePro")]
public class bl_NamePlateProSettings : ScriptableObject
{
    public enum HitEffect
    {
        None = 0,
        Delayed = 1,
        ColorFade = 2,
    }

    [Tooltip("Add an offset to the name plater vertical position.")]
    public float verticalPosOffset = 0.85f;
    [Tooltip("The max distance to show the name plate text from the local player position to the target position.")]
    public float minTagDisplayDistance = 30f;
    [Tooltip("The max distance to show the name plate UI (not just the text but the position indicator too)")]
    public float maxDisplayDistance = 100f;
    [Tooltip("Define how much adjust the vertical position of the name plater based on the distance of the target.")]
    public float verticalAdjustment = 0.13f;
    [Tooltip("Show the health bar in the name plate?")]
    [LovattoToogle] public bool showHealth = true;
    [Tooltip("Does the camera field of view affect the distance calculation?")]
    [LovattoToogle] public bool distanceBasedFOV = true;
    [Tooltip("Effect that shows when the player receive damage")]
    public HitEffect hitEffect = HitEffect.None;
}