using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.FloatingTextAsset
{
    public class bl_FloatingTextCamera : MonoBehaviour
    {
        private Camera cam;

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            if (TryGetComponent(out cam))
            {
                if (bl_FloatingTextManager.Instance != null)
                    bl_FloatingTextManager.Instance.PlayerCamera = cam;
            }
        }
    }
}