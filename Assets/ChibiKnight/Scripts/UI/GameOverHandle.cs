using ChibiKnight.Systems.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;
using ChibiKnight.Systems;

namespace ChibiKnight
{
    public class GameOverHandle : MonoBehaviour
    {
        [SerializeField]
        private Damageable m_player;
        [SerializeField]
        private EnemyWaveSpawnHandle m_spawnHandle;
        private void OnGameEnded()
        {
            GameEventMessage.SendEvent("Game Over");
        }

        private void Awake()
        {
            m_player.OnDeath += OnGameEnded;
            m_spawnHandle.AllWavesDefeated += OnGameEnded;
        }

    }
}