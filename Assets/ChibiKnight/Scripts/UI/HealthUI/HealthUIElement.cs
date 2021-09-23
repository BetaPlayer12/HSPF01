using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChibiKnight.Gameplay.UI
{
    public class HealthUIElement : MonoBehaviour
    {
        [SerializeField]
        private Image m_ui;
        [SerializeField, MinValue(1), DisableInPlayMode]
        private int m_maxHealthValueRepresentation = 1;
        [SerializeField, InfoBox("Should Have same element in m_maxHealthValueRepresentation")]
        private Sprite[] m_valueRepresentation;

        public int maxHealthValueRepresentation => m_maxHealthValueRepresentation;

        public void SetValue(int value)
        {
            m_ui.sprite = m_valueRepresentation[value];
        }
    }

}