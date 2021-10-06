using System;
using UnityEngine;

namespace ChibiKnight.Systems.Pooling
{
    public class PoolableObject : MonoBehaviour
    {
        [SerializeField]
        private PoolID m_poolID;

        public event Action<PoolableObject> PoolRequested;
        public event Action Spawned;

        public PoolID poolID => m_poolID;

        public void SpawnAt(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            Spawned?.Invoke();
        }

        public void Release()
        {
            PoolRequested?.Invoke(this);
        }
    }
}