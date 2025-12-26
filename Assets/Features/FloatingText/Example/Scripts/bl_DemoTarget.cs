using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lovatto.FloatingTextAsset
{
    public class bl_DemoTarget : MonoBehaviour
    {
        public UEvent onHit;
       
        [System.Serializable]public class UEvent : UnityEvent<RaycastHit> { }
        private Material mat;

        private void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
        }

        public void OnHit(RaycastHit hitTransform)
        {
            onHit?.Invoke(hitTransform);
            StopAllCoroutines();
            StartCoroutine(HitEffect());
        }

        IEnumerator HitEffect()
        {
            float d = 0;
            Color color;
            while(d < 1)
            {
                d += Time.deltaTime * 3.3f;
                color = Color.Lerp(Color.black, Color.red, bl_DemoScene.Instance.hitEffectCurve.Evaluate(d));
                mat.SetVector("_EmissionColor", color);
                yield return null;
            }
        }
    }
}