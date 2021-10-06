using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiKnight.Systems.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField]
        private PoolListData m_poolData;
        [SerializeField]
        private Transform m_poolParent;
        [SerializeField, DisableInPlayMode, DisableInEditorMode]
        private List<PoolableObject> m_poolList;

        public static PoolManager instance { get; private set; }

        public bool GetInstanceOf(PoolID poolID, out PoolableObject instance)
        {
            instance = null;
            for (int i = 0; i < m_poolList.Count; i++)
            {
                var poolElement = m_poolList[i];
                if (poolElement.poolID == poolID)
                {
                    instance = poolElement;
                    instance.transform.SetParent(null);
                    m_poolList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void OnPoolRequest(PoolableObject obj)
        {
            obj.transform.SetParent(m_poolParent);
            m_poolList.Add(obj);
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < m_poolData.count; i++)
                {
                    var info = m_poolData.GetInfo(i);
                    for (int j = 0; j < info.instanceCount; j++)
                    {
                        var instance = Instantiate(info.poolable, m_poolParent);
                        var poolableObject = instance.GetComponent<PoolableObject>();
                        poolableObject.PoolRequested += OnPoolRequest;
                        m_poolList.Add(poolableObject);
                    }
                }
            }
        }
    }
}