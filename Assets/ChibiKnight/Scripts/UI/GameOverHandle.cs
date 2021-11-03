using ChibiKnight.Systems.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;
using ChibiKnight.Systems;
using UnityEngine.SceneManagement;

namespace ChibiKnight
{
    public class GameOverHandle : MonoBehaviour
    {
        [SerializeField]
        private Damageable m_player;
        [SerializeField]
        private EnemyWaveSpawnHandle m_spawnHandle;
        [SerializeField]
        private float m_showMessageDuration;

        private void OnGameEnded()
        {
            GameEventMessage.SendEvent("Game Over");
            StartCoroutine(GoToMainMenuRoutine());
        }

        private IEnumerator GoToMainMenuRoutine()
        {
            yield return new WaitForSecondsRealtime(m_showMessageDuration);
            SceneManager.LoadScene(0);
        }


        private void Awake()
        {
            m_player.OnDeath += OnGameEnded;
            m_spawnHandle.AllWavesDefeated += OnGameEnded;
        }

    }
}