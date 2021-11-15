using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChibiKnight.UI
{
    public class UITextHighlight : MonoBehaviour
    {
        [SerializeField]
        private Color m_highlightColor = Color.white;
        [SerializeField]
        private Color m_nonHighlightColor = Color.white;

        private TextMeshProUGUI m_text;

        public void UseHighlightColor()
        {
            m_text.color = m_highlightColor;
        }

        public void UseNonHighlightColor()
        {
            m_text.color = m_nonHighlightColor;
        }

        private void Awake()
        {
            m_text = GetComponent<TextMeshProUGUI>();
        }
    }
}