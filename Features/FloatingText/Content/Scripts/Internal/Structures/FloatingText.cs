using System;
using UnityEngine;
using Lovatto.FloatingTextAsset;

/// <summary>
/// Structure to build and show a floating text
/// Build the floating text using the constructor and the Set... function
/// Finally show the floating text by calling this.Show();
/// </summary>
public struct FloatingText
{
    public Transform Target { get; set; }
    public Vector3 Position { get; set; }
    public string Text { get; set; }
    public Color TextColor { get; private set; }
    public Vector3 PositionOffset { get; private set; }
    public int ExtraTextSize { get; private set; }
    public int ReuseTimes { get; private set; }
    public float OutlineSize { get; private set; }
    public Color OutlineColor { get; private set; }
    public FloatingTextSettings Settings { get; private set; }
    public Action FinishCallback { get; private set; }
    public FloatingTextFlags Flags { get; private set; }

    public InternalProps InternalOnly;

    public struct InternalProps
    {
        public Vector3 InitRectPosition { get; set; }
        public Vector2 FloatDirection { get; set; }
        public bool InvertHorizontalDirection { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    public FloatingText(string text)
    {
        Target = null;
        Position = Vector3.zero;
        Text = text;

        TextColor = Color.white;
        PositionOffset = Vector3.zero;
        ExtraTextSize = 0;
        ReuseTimes = 0;
        Settings = null;
        OutlineColor = Color.clear;
        OutlineSize = -1;
        InternalOnly = new InternalProps();
        FinishCallback = null;
        Flags = FloatingTextFlags.None;
    }

    /// <summary>
    /// Prepare and show the floating text on the screen
    /// </summary>
    public void Show()
    {
        if (string.IsNullOrEmpty(Text)) return;

        if (Settings == null) Settings = bl_FloatingTextManagerSettings.Instance.floatingTextSettings[0].Settings;
        bl_FloatingTextManager.Instance.InstanceFloatingText(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosition() => Position + PositionOffset;

    /// <summary>
    /// Set floating text target
    /// </summary>
    public FloatingText SetTarget(Transform target)
    {
        Target = target;
        if (Target != null) Position = Target.position;
        return this;
    }

    /// <summary>
    /// Set floating text start position
    /// </summary>
    public FloatingText SetPosition(Vector3 position)
    {
        Position = position;
        return this;
    }

    /// <summary>
    /// Set text color
    /// </summary>
    public FloatingText SetTextColor(Color textColor)
    {
        TextColor = textColor;
        return this;
    }

    /// <summary>
    /// Set the start position offset
    /// </summary>
    public FloatingText SetPositionOffset(Vector3 offset)
    {
        PositionOffset = offset;
        return this;
    }

    /// <summary>
    /// Apply an extra size to the text
    /// </summary>
    public FloatingText SetExtraTextSize(int size)
    {
        ExtraTextSize = size;
        return this;
    }

    /// <summary>
    /// Set reuses times
    /// Means how many times a text will use the same text instance instead of create a new one
    /// when the floating text is create for the same target within a short period of time.
    /// </summary>
    public FloatingText SetReuses(int reuses)
    {
        ReuseTimes = reuses;
        return this;
    }

    /// <summary>
    /// Set the floating text settings
    /// </summary>
    public FloatingText SetSettings(string settingsName)
    {
        SetSettings(bl_FloatingTextManagerSettings.Instance.GetSettings(settingsName));
        return this;
    }

    /// <summary>
    /// Set the floating text settings
    /// </summary>
    public FloatingText SetSettings(FloatingTextSettings settings)
    {
        Settings = settings;
        return this;
    }

    /// <summary>
    /// Set text outline size
    /// </summary>
    public FloatingText SetOutlineSize(float size)
    {
        OutlineSize = size;
        return this;
    }

    /// <summary>
    /// Set Text outline color
    /// </summary>
    public FloatingText SetOutlineColor(Color color)
    {
        OutlineColor = color;
        return this;
    }

    /// <summary>
    /// Make reuse this instance while the text is showing
    /// </summary>
    public FloatingText ReuseWhileAlive()
    {
        ReuseTimes = -2;
        return this;
    }

    /// <summary>
    /// Don't replay the sequence/floating when re-used this instance
    /// </summary>
    public FloatingText DontRewindOnReuse()
    {
        Flags |= FloatingTextFlags.DontRewind;
        return this;
    }

    /// <summary>
    /// Make this text stick at the original world position and not to the screen position
    /// </summary>
    public FloatingText StickAtOriginWorldPosition()
    {
        Flags |= FloatingTextFlags.StickAtOriginPosition;
        return this;
    }

    /// <summary>
    /// Make this text stick at the original world position and not to the screen position
    /// </summary>
    public FloatingText InvertHorizontalDirectionRandomly()
    {
        Flags |= FloatingTextFlags.InvertHorizontalDirectionRandomly;
        return this;
    }

    /// <summary>
    /// Add a callback that will be invoke once the floating text sequence finish/hide.
    /// </summary>
    /// <param name="callback"></param>
    public FloatingText OnFinish(Action callback)
    {
        FinishCallback += callback;
        return this;
    }
}