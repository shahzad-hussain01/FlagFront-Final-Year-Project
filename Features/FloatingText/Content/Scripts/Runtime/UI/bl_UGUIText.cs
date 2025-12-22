using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lovatto.FloatingTextAsset
{
    public class bl_UGUIText : UnityText
    {
        public Text m_text;
        public Outline m_outline;
        public Shadow m_shadow;

        public override string text { get => m_text.text; set => m_text.text = value; }
        public override Color color { get => m_text.color; set => m_text.color = value; }
        public override float fontSize { get => m_text.fontSize; set => m_text.fontSize = (int)value; }
        public override float outlineSize
        {
            get => m_outline.effectDistance.x; set
            {
               
                if (m_outline != null) m_outline.effectDistance = Vector2.one * value;
                if (m_shadow != null) m_shadow.effectDistance = Vector2.one * value;
            }
        }
        public override Color outlineColor
        {
            get => m_outline.effectColor; 
            set
            {
              if(m_outline != null)  m_outline.effectColor = value;
                if (m_shadow != null) m_shadow.effectColor = value;
            }
        }
    }
}