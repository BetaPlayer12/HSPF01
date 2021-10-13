using ChibiKnight.Systems.Combat;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiKnight.Systems
{
    public class EnemyWaveSpawnHandle : MonoBehaviour
    {
        [System.Serializable]
        private class WaveHandle
        {
            [SerializeField]
            private GameObject[] m_minions;

            private bool m_eventsListened;
            private int minionCount;

            public Action WaveEnded;

            public void Initialize()
            {
                for (int i = 0; i < m_minions.Length; i++)
                {
                    m_minions[i].GetComponent<Damageable>().OnDeath += OnMinionDeath;
                }
            }


            public void StartWave()
            {
                for (int i = 0; i < m_minions.Length; i++)
                {
                    m_minions[i].SetActive(true);
                }
            }

            public void StopWave()
            {
                for (int i = 0; i < m_minions.Length; i++)
                {
                    m_minions[i].SetActive(true);
                }
            }
            private void OnMinionDeath()
            {
                minionCount--;
                if (minionCount <= 0)
                {
                    WaveEnded?.Invoke();
                }
            }
        }

        [SerializeField]
        private WaveHandle[] m_waves;
        private int m_currentWaveIndex;

        public event Action AllWavesDefeated;
        
        [Button,HideInEditorMode,PropertyOrder(-1)]
        public void StartWaveSpawning()
        {
            m_currentWaveIndex = 0;
            m_waves[m_currentWaveIndex].StartWave();
        }
        
        private void OnWaveEnded()
        {
            m_currentWaveIndex++;
            if (m_currentWaveIndex < m_waves.Length)
            {
                m_waves[m_currentWaveIndex].StartWave();
            }
            else
            {
                AllWavesDefeated?.Invoke();
            }
        }

        private void Start()
        {
            for (int i = 0; i < m_waves.Length; i++)
            {
                var wave = m_waves[i];
                wave.WaveEnded = OnWaveEnded;
                wave.Initialize();
                wave.StopWave();
            }
        }

    }

}