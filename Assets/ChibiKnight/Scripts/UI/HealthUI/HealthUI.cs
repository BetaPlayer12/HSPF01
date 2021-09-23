using ChibiKnight.Systems.Combat;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChibiKnight.Gameplay.UI
{
    public class HealthUI : SerializedMonoBehaviour
    {
        [SerializeField]
        private IHealth m_toTrack;
        [SerializeField]
        private GameObject m_template;
        [SerializeField]
        private GridLayoutGroup m_gridLayout;
        [SerializeField]
        private List<HealthUIElement> m_elements;

        private void Awake()
        {
            m_gridLayout.enabled = true;
            m_toTrack.HealthChange += OnHealthChange;
            var healthValueWithoutUI = m_toTrack.maxHealth;
            for (int i = 0; i < m_elements.Count; i++)
            {
                var element = m_elements[i];
                var elementValue = Mathf.Min(element.maxHealthValueRepresentation, healthValueWithoutUI);
                element.SetValue(elementValue);
                healthValueWithoutUI -= elementValue;
            }

            if (healthValueWithoutUI > 0)
            {
                do
                {
                    var uiElement = Instantiate(m_template, transform).GetComponent<HealthUIElement>();
                    var elementValue = Mathf.Min(uiElement.maxHealthValueRepresentation, healthValueWithoutUI);
                    uiElement.SetValue(elementValue);
                    healthValueWithoutUI -= elementValue;
                    m_elements.Add(uiElement);
                } while (healthValueWithoutUI > 0);
            }
        }

        private void Start()
        {
            m_gridLayout.enabled = false;
        }

        private void OnHealthChange(int value)
        {
            for (int i = 0; i < m_elements.Count; i++)
            {
                var element = m_elements[i];
                var elementValue = Mathf.Min(element.maxHealthValueRepresentation, value);
                value -= elementValue;
                element.SetValue(elementValue);
            }
        }
    }

}