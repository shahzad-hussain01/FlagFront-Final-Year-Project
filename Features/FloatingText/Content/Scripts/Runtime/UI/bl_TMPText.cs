using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Lovatto.FloatingTextAsset
{
    public class bl_TMPText : UnityText
    {
        public TextMeshProUGUI m_text;
        public bool useUnderlay = false;
        public float outlineDivider = 6;


        public override string text { get => m_text.text; set => m_text.text = value; }
        public override Color color { get => m_text.color; set => m_text.color = value; }
        public override float fontSize { get => m_text.fontSize; set => m_text.fontSize = value; }
        public override float outlineSize
        {
            get => m_text.outlineWidth; set
            {
                if (!useUnderlay) m_text.outlineWidth = value / outlineDivider;
            }
        }
        public override Color outlineColor
        {
            get => m_text.outlineColor;
            set
            {
                if (!useUnderlay) m_text.outlineColor = value;
                else
                {
                    m_text.fontMaterial.SetColor("_UnderlayColor", value);
                }
            }
        }
    }
}