using Sirenix.OdinInspector;
using UnityEngine;

namespace ChibiKnight.Systems.Pooling
{
    [CreateAssetMenu(fileName = "PoolListData", menuName = "ChibiKnight/PoolListData")]
    public class PoolListData : ScriptableObject
    {
        [System.Serializable]
        public class SetupInfo
        {
            [SerializeField]
            private GameObject m_poolable;
            [SerializeField, MinValue(1)]
            private int m_instanceCount = 1;

            public GameObject poolable => m_poolable;
            public int instanceCount => m_instanceCount;
        }

        [SerializeField, TableList]
        private SetupInfo[] m_poolList;

        public int count => m_poolList.Length;
        public SetupInfo GetInfo(int index) => m_poolList[index];
    }
}