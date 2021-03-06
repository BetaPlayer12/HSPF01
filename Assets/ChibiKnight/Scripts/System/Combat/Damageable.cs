using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiKnight.Systems.Combat
{
    public class Damageable : MonoBehaviour, IHealth
    {
        [SerializeField, MinValue(1)]
        private int m_maxHealth = 1;
        [ShowInInspector, HideInEditorMode, MinValue(0), MaxValue("m_maxHealth"), OnValueChanged("OnEditorHealthChange")]
        private int m_currentHealth;

        public int maxHealth => m_currentHealth;

        public event Action<int> HealthChange;
        public event Action OnDeath;
        public event Action<Vector3> OnDeath2;
        public event Action<Vector3> OnTakeDamage;
        public int currentHealth
        {
            get => m_currentHealth;
            private set
            {
                m_currentHealth = value;
                HealthChange?.Invoke(m_currentHealth);
            }
        }


        public void TakeDamage(int value, Vector2 damageSource)
        {
            currentHealth = Mathf.Max(0, currentHealth - value);

            if (currentHealth <= 0)
            {
                OnDeath?.Invoke();
                OnDeath2?.Invoke(damageSource);
            }
            else
            {
                OnTakeDamage?.Invoke(damageSource);
            }
        }

        private void OnEditorHealthChange()
        {
            HealthChange?.Invoke(m_currentHealth);
        }

        public void Reset()
        {
            currentHealth = m_maxHealth;
        }

        private void Awake()
        {
            Reset();
        }

    }
}