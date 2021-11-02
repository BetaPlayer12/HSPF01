using ChibiKnight.Systems;
using Doozy.Engine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChibiKnight.Gameplay
{
    public class WaveAnnouncerUI : MonoBehaviour
    {
        [SerializeField]
        private EnemyWaveSpawnHandle m_spawnHandle;
        [SerializeField]
        private Image m_image;
        [SerializeField]
        private Sprite[] m_spriteChange;


        private void Awake()
        {
            
        }
        
        [Button]
        public void ShowAnnouncement()
        {
            GameEventMessage.SendEvent("New Wave");
        }
    }

}