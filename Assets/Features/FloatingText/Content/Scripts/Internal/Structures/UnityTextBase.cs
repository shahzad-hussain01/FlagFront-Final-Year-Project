using UnityEngine;

namespace Lovatto.FloatingTextAsset
{
    public abstract class UnityText : MonoBehaviour
    {
        public abstract string text
        {
            get; set;
        }

        public abstract Color color
        {
            get; set;
        }

        public abstract float fontSize
        {
            get; set;
        }

        public abstract float outlineSize
        {
            get; set;
        }

        public abstract Color outlineColor
        {
            get; set;
        }
    }
}