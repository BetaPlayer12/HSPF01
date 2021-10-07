using DChild.Gameplay.Characters;
using Holysoft.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DChild.Gameplay
{

    [SelectionBase]
    [AddComponentMenu("DChild/Gameplay/Objects/Character")]
    public class Character : MonoBehaviour, ITurningCharacter
    {
        public static string objectTag => "Character";

        [SerializeField, MinValue(0.5f)]
        private float m_height;
        [SerializeField]
        private Transform m_centerMass;
        [SerializeField]
        private Rigidbody2D m_physics;
        [SerializeField]
        private HorizontalDirection m_facing = HorizontalDirection.Right;

        private int m_ID;
        private bool m_hasID;

        public event EventAction<FacingEventArgs> CharacterTurn;

        public float height => m_height;
        public HorizontalDirection facing => m_facing;

        public Transform centerMass => m_centerMass;

        public int ID => m_ID;
        public bool hasID => m_hasID;

        public void SetID(int ID)
        {
            m_ID = ID;
            m_hasID = true;
        }

        public void SetFacing(HorizontalDirection facing)
        {
            m_facing = facing;
            CharacterTurn?.Invoke(this, new FacingEventArgs(m_facing));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * m_height));
        }

        private void OnValidate()
        {
            if (gameObject.CompareTag(objectTag) == false)
            {
                gameObject.tag = objectTag;
                Debug.Log(gameObject.tag);
            }
        }

#if UNITY_EDITOR
        public void InitializeField(Transform centermass, Rigidbody2D physics)
        {
            m_centerMass = centermass;
            m_physics = physics;
        }
#endif
    }
}