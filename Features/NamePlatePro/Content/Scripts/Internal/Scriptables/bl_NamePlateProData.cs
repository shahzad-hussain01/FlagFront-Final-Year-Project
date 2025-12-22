using MFPS.Addon.NamePlatePro;
using UnityEngine;

[CreateAssetMenu(fileName = "NamePlateProData", menuName = "MFPS/Settings/NamePlateData")]
public class bl_NamePlateProData : ScriptableObject
{

    [SerializeField] private GameObject namePlateCanvas = null;
    [SerializeField] private bl_NamePlaterProUIBase namePlatePrefab = null;

    /// <summary>
    /// 
    /// </summary>
    public static void InitInScene()
    {
        if (bl_NamePlateProManager.Instance != null) return;

        GameObject g = Instantiate(Instance.namePlateCanvas);
        g.name = Instance.namePlateCanvas.name;

        var script = g.GetComponentInChildren<bl_NamePlateProManager>();
        script.platePrefab = Instance.namePlatePrefab;
    }

    private static bl_NamePlateProData m_Data;
    public static bl_NamePlateProData Instance
    {
        get
        {
            if (m_Data == null)
            {
                m_Data = Resources.Load("NamePlateProData", typeof(bl_NamePlateProData)) as bl_NamePlateProData;
            }
            return m_Data;
        }
    }
}