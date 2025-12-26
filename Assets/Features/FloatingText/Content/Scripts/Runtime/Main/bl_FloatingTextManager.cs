using System.Collections.Generic;
using UnityEngine;
using Lovatto.FloatingTextAsset;
using UnityEngine.Events;

public class bl_FloatingTextManager : MonoBehaviour
{
    #region Public members
    [Header("Settings")]
    public int maxTextsInstances = 30;
    public UEvent onNewText;

    [Header("References")]
    public RectTransform parentReference;
    [SerializeField] private Camera m_playerCamera;
    public Camera PlayerCamera
    {
        get => m_playerCamera;
        set => m_playerCamera = value;
    }
    [SerializeField] private bl_FloatingText textTemplate = null;
    #endregion

    #region Private members   
    private List<bl_FloatingText> textsPool = new List<bl_FloatingText>();
    private Dictionary<Transform, bl_FloatingText> reusedTexts = new Dictionary<Transform, bl_FloatingText>();
    private int currentPoolID = 0;
    [System.Serializable]public class UEvent : UnityEvent { }
    #endregion

    /// <summary>
    /// Instance a new floating text with the given data
    /// </summary>
    /// <param name="data"></param>
    public void InstanceFloatingText(FloatingText data)
    {
        bl_FloatingText floatingText = GetTextInstance(data);
        floatingText.Set(data);
        onNewText?.Invoke();
    }

    /// <summary>
    /// Pool text instance to instantiate only the required texts
    /// </summary>
    /// <returns></returns>
    private bl_FloatingText GetTextInstance(FloatingText data)
    {
        if((data.ReuseTimes > 0 || data.ReuseTimes == -2) && data.Target != null)
        {
            if (reusedTexts.ContainsKey(data.Target))
            {
                var t = reusedTexts[data.Target];
                if (t.UseTimes > 0)
                {
                    t.UseTimes--;
                    return t;
                }
                else if (t.UseTimes == -2) return t;
            }
        }

        if (textsPool.Count >= maxTextsInstances)
        {
            var t = textsPool[currentPoolID];
            currentPoolID = (currentPoolID + 1) % maxTextsInstances;
            return t;
        }
        else
        {
            for (int i = 0; i < textsPool.Count; i++)
            {
                if (!textsPool[i].IsActive()) return textsPool[i];
            }

            var go = Instantiate(textTemplate.gameObject) as GameObject;
            go.transform.SetParent(parentReference, false);

            var script = go.GetComponent<bl_FloatingText>();
            textsPool.Add(script);
            return script;
        }
    }

    /// <summary>
    /// Change the floating text prefab
    /// this will clear all the pool instances.
    /// </summary>
    public void ChangeTextPrefab(bl_FloatingText newPrefab)
    {
        if (newPrefab == textTemplate) return;

        textsPool.ForEach(x =>
        {
            if (x != null) Destroy(x.gameObject);
        });
        textsPool = new List<bl_FloatingText>();
        reusedTexts = new Dictionary<Transform, bl_FloatingText>();
        currentPoolID = 0;
        textTemplate = newPrefab;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="floatingText"></param>
    public void AddToReused(bl_FloatingText floatingText, FloatingText data)
    {
        if (reusedTexts.ContainsKey(data.Target)) return;
        reusedTexts.Add(data.Target, floatingText);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="floatingText"></param>
    public void RemoveFromReused(FloatingText data)
    {
        if (!reusedTexts.ContainsKey(data.Target)) return;
        reusedTexts.Remove(data.Target);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wPosition"></param>
    /// <returns></returns>
    public static Vector3 GetScreenPositionFromWorldPosition(Vector3 wPosition)
    {
        var cam = Instance.PlayerCamera;
        if (cam == null) cam = Camera.current;
        if (cam == null) return Vector3.zero;

        return cam.WorldToScreenPoint(wPosition);
    }

    private Canvas m_canvas;
    public Canvas UsedCanvas
    {
        get
        {
            if (m_canvas == null) m_canvas = transform.GetComponentInParent<Canvas>();
            return m_canvas;
        }
    }

    private static bl_FloatingTextManager _instance;
    public static bl_FloatingTextManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<bl_FloatingTextManager>();
            return _instance;
        }
    }
}