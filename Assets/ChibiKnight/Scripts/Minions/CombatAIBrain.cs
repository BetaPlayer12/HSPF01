using ChibiKnight.Gameplay;
using ChibiKnight.Systems.Combat;
using Holysoft.Event;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace DChild.Gameplay.Characters.AI
{
    public abstract class CombatAIBrain : MonoBehaviour
    {
        [Flags]
        protected enum Restriction
        {
            ForbiddenFromAttackingTarget = 1 << 0,
            IgnoreTarget = 1 << 1
        }

        [SerializeField, TabGroup("Reference")]
        protected Character m_character;
        [SerializeField, TabGroup("Reference")]
        protected SkeletonAnimationHelper m_animation;
        [SerializeField, TabGroup("Reference")]
        protected Damageable m_damageable;
        [SerializeField, TabGroup("Reference")]
        protected FlinchHolder m_flinch;
        [SerializeField, TabGroup("Reference")]
        protected Transform m_centerMass;
        [SerializeField, TabGroup("Data")]
        protected CharacterState m_state;
        [SerializeField, TabGroup("Modules")]
        protected Rigidbody2D m_physics;

        [SerializeField, TabGroup("Physics")]
        protected PhysicsMaterial2D m_groundedPhysicsMaterial;
        [SerializeField, TabGroup("Physics")]
        protected PhysicsMaterial2D m_midairPhysicsMaterial;
        [SerializeField, TabGroup("Physics")]
        private LayerMask m_collisionLayer;
        [SerializeField, TabGroup("Physics")]
        private Vector2 m_origin;
        [SerializeField, TabGroup("Physics")]
        private Vector2 boxSize;
        [SerializeField, TabGroup("Physics")]
        private float angle;
        [SerializeField, TabGroup("Physics")]
        private float m_groundCheckOffset;

        private Transform m_target;

        private ContactFilter2D m_filter;
        private List<Collider2D> m_colliderList;

        private Attacker m_attacker;

        protected Restriction m_currentRestrictions;

        private void Initialize()
        {
            //Ground Check Stuff
            m_filter = new ContactFilter2D();
            m_filter.useTriggers = false;
            m_filter.useLayerMask = true;
            m_filter.layerMask = m_collisionLayer;
            m_colliderList = new List<Collider2D>();
        }


        protected void EvaluateGroundedness()
        {
            int groundColliderResult = Physics2D.OverlapBox(m_origin + (Vector2)transform.position, boxSize, angle, m_filter, m_colliderList);
            var isGrounded = groundColliderResult > 0 ? true : false;

            m_state.isGrounded = isGrounded;

            if (isGrounded == true)
            {
                m_physics.sharedMaterial = m_groundedPhysicsMaterial;
                //m_state.isHighJumping = false;
            }
            else
            {
                m_physics.sharedMaterial = m_midairPhysicsMaterial;
            }
        }


        public virtual void Enable()
        {
            enabled = true;
        }

        public virtual void Disable()
        {
            enabled = false;
        }

        private void CustomTurn()
        {
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            m_character.SetFacing(transform.localScale.x == 1 ? HorizontalDirection.Right : HorizontalDirection.Left);
        }

        public bool IsFacing(Vector2 position)
        {
            if (position.x > m_character.transform.position.x)
            {
                return m_character.facing == HorizontalDirection.Right;
            }
            else
            {
                return m_character.facing == HorizontalDirection.Left;
            }
        }

        /// <summary>
        /// AI Essentially Resets as its Returns to Spawn Point
        /// </summary>

        /// <summary>
        /// Will still be Notice the Target but cannot attack it
        /// </summary>
        /// <param name="value"></param>
        public void ForbidFromAttackTarget(bool value)
        {
            if (value)
            {
                m_currentRestrictions |= Restriction.IgnoreTarget;
            }
            else
            {
                m_currentRestrictions |= Restriction.IgnoreTarget;

            }
        }

        protected bool HasRestriction(Restriction restriction) => m_currentRestrictions.HasFlag(restriction);

        protected bool IsFacingTarget() => IsFacing(m_target.position);
        protected bool IsTargetInRange(float distance) => Vector2.Distance(m_target.position, m_character.centerMass.position) <= distance;
        protected Vector2 DirectionToTarget() => (m_target.position - m_character.transform.position).normalized;

        public virtual void SetTarget(Transform target)
        {
            m_target = target;
        }

        protected virtual void Start()
        {
            Initialize();
        }

        protected virtual void Awake()
        {
            m_damageable.OnDeath += OnDestroyed;
            m_attacker = GetComponent<Attacker>();
        }

        protected virtual void OnDestroyed() => base.enabled = false;

#if UNITY_EDITOR

        public void InitializeField(Character character, SkeletonAnimationHelper spineRoot, Damageable damageable, Transform centerMass)
        {
            m_character = character;
            m_animation = spineRoot;
            m_damageable = damageable;
            m_centerMass = centerMass;
        }


#endif
    }
}