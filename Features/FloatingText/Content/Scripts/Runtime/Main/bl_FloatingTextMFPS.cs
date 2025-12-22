using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.FloatingTextAsset
{
    public class bl_FloatingTextMFPS : MonoBehaviour
    {
        Vector3 offset = Vector3.zero;

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            bl_EventHandler.onLocalPlayerSpawn += OnLocalSpawn;
            bl_EventHandler.onLocalPlayerHitEnemy += OnLocalHitEnemy;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            bl_EventHandler.onLocalPlayerSpawn -= OnLocalSpawn;
            bl_EventHandler.onLocalPlayerHitEnemy -= OnLocalHitEnemy;
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalSpawn()
        {
            bl_FloatingTextManager.Instance.PlayerCamera = bl_MFPS.LocalPlayerReferences.playerCamera;
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalHitEnemy(MFPSHitData hitData)
        {
            var fts = bl_FloatingTextManagerSettings.Instance;
            if (bl_MFPS.LocalPlayerReferences != null)
            {
                offset = bl_MFPS.LocalPlayerReferences.transform.TransformDirection(fts.damagePositionOffset);
            }
            else offset = Vector3.zero;

            float criticDamage = (float)hitData.Damage / fts.criticalDamage;
            criticDamage = Mathf.Clamp01(criticDamage);
            var textColor = fts.damageTextColorGradient.Evaluate(criticDamage);

            new FloatingText(string.Format(fts.damageTextFormat, hitData.Damage))
                     .SetTextColor(textColor)
                     .SetPosition(hitData.HitPosition)
                     .SetTarget(hitData.HitTransform)
                     .SetPositionOffset(offset)
                     .StickAtOriginWorldPosition()
                     .SetSettings(fts.damageTextSetting)
                     .SetExtraTextSize(fts.extraTextSize)
                     .InvertHorizontalDirectionRandomly()
                     .SetReuses(fts.textReuses)
                     .Show();
        }
    }
}