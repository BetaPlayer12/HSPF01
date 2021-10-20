using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using DChild.Gameplay.Characters.AI;
using ChibiKnight.Systems.Combat;
using Holysoft.Event;
using Spine.Unity;
using System;

namespace DChild.Gameplay.Characters.Enemies
{
    public class ChibiBlobAI : CombatAIBrain
    {
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_idleAnimation;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_attackAnimation;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_deathAnimation;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_moveAnimation;

        [SerializeField, TabGroup("AttackStats")]
        private float m_attackRange;
        [SerializeField, TabGroup("AttackStats")]
        private float m_attackTime;

        private enum State
        {
            Detect,
            Idle,
            Attacking,
            Cooldown,
            Chasing,
            WaitBehaviourEnd,
        }

        private enum Attack
        {
            Attack,
            [HideInInspector]
            _COUNT
        }
        
        [SerializeField, TabGroup("Modules")]
        private SkeletonRootMotion m_rootMotion;
        [SerializeField, TabGroup("Modules")]
        private AnimatedTurnHandle m_turnHandle;
        [SerializeField, TabGroup("Hitbox")]
        private Collider2D m_hitbox;
        [SerializeField, TabGroup("Hurtbox")]
        private Collider2D m_explosionBB;
        [SerializeField, TabGroup("Hurtbox")]
        private Collider2D m_attackBB;

        private float m_currentCD;
        private float m_currentFullCD;

        [ShowInInspector]
        private StateHandle<State> m_stateHandle;

        private Coroutine m_attackRoutine;

        private void OnAttackDone(object sender, EventActionArgs eventArgs)
        {
            //m_animation.DisableRootMotion();
            m_stateHandle.ApplyQueuedState();
        }

        private void OnTurnDone(object sender, FacingEventArgs eventArgs)
        {
            m_stateHandle.ApplyQueuedState();
        }

        private void CustomTurn()
        {
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            m_character.SetFacing(transform.localScale.x == 1 ? HorizontalDirection.Right : HorizontalDirection.Left);
        }


        //protected override void OnDestroyed(object sender, EventActionArgs eventArgs)
        //{
        //    //m_Audiosource.clip = m_DeadClip;
        //    //m_Audiosource.Play();
        //    StopAllCoroutines();
        //    base.OnDestroyed(sender, eventArgs);
        //    m_stateHandle.OverrideState(State.WaitBehaviourEnd);
        //    if (m_attackRoutine != null)
        //    {
        //        StopCoroutine(m_attackRoutine);
        //    }
        //    if (m_sneerRoutine != null)
        //    {
        //        StopCoroutine(m_sneerRoutine);
        //    }
        //    m_character.physics.UseStepClimb(true);
        //    m_movement.Stop();
        //    m_animation.SetEmptyAnimation(0, 0);
        //    StartCoroutine(ResurrectRoutine());
        //}

        private void OnFlinchStart(object sender, EventActionArgs eventArgs)
        {
            if (!IsFacingTarget())
                CustomTurn();

            m_stateHandle.Wait(State.Chasing);
        }

        private void OnFlinchEnd(object sender, EventActionArgs eventArgs)
        {
            m_stateHandle.ApplyQueuedState();
        }

        private IEnumerator DetectRoutine()
        {
            m_animation.SetAnimation(0, m_idleAnimation, true);
            yield return new WaitForAnimationComplete(m_animation.animationState, m_idleAnimation);
            m_stateHandle.ApplyQueuedState();
            yield return null;
        }

        private IEnumerator AttackRoutine()
        {
            m_animation.SetAnimation(0, m_attackAnimation, false).MixDuration = 0;
            m_attackBB.enabled = true;
            //yield return new WaitForSeconds(.75f);
            var target = m_target.position;
            //while (Vector2.Distance(transform.position, target) > 1f)
            //{
            //    m_physics.velocity = new Vector2(transform.right.x * m_attackMoveSpeed, m_physics.velocity.y);
            //    yield return null;
            //}
            var targetDistance = Vector2.Distance(target, transform.position);
            var velocity = (targetDistance / m_attackTime) * transform.localScale.x;
            float time = 0;
            float animTime = 1 / (m_attackTime / 0.75f);
            m_animation.animationState.TimeScale = animTime;
            m_rootMotion.enabled = false;
            while (time < m_attackTime)
            {
                m_physics.velocity = new Vector2(velocity, m_physics.velocity.y);
                time += Time.deltaTime;
                yield return null;
            }
            m_rootMotion.enabled = true;
            m_physics.velocity = Vector2.zero;
            m_attackBB.enabled = false;
            m_animation.animationState.TimeScale = 1;
            yield return new WaitForAnimationComplete(m_animation.animationState, m_attackAnimation);
            m_stateHandle.ApplyQueuedState();
            yield return null;
        }

        private IEnumerator DeathRoutine()
        {
            m_animation.SetAnimation(0, m_deathAnimation, false);
            yield return new WaitForSeconds(.25f);
            m_explosionBB.enabled = true;
            yield return new WaitForSeconds(.25f);
            m_explosionBB.enabled = false;
            yield return new WaitForAnimationComplete(m_animation.animationState, m_deathAnimation);
            this.gameObject.SetActive(false);
            yield return null;
        }

        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            m_stateHandle.SetState(State.Detect);
        }

        protected override void Start()
        {
            base.Start();
            m_flinch.CanFlinch(false);
            m_currentFullCD = UnityEngine.Random.Range(.5f, 2f);
        }

        protected override void Awake()
        {
            base.Awake();
            m_turnHandle.TurnDone += OnTurnDone;
            m_flinch.FlinchStart += OnFlinchStart;
            m_flinch.FlinchEnd += OnFlinchEnd;
            m_damageable.OnDeath += Death;
            m_stateHandle = new StateHandle<State>(State.Idle, State.WaitBehaviourEnd);
        }

        private void Death()
        {
            m_hitbox.enabled = false;
            m_flinch.gameObject.SetActive(false);
            enabled = false;
            m_animation.SetEmptyAnimation(0, 0);
            m_animation.SetEmptyAnimation(1, 0);
            m_stateHandle.Wait(State.Detect);
            if (m_attackRoutine != null)
            {
                StopCoroutine(m_attackRoutine);
            }
            StartCoroutine(DeathRoutine());
        }

        private void Update()
        {
            EvaluateGroundedness();

            switch (m_stateHandle.currentState)
            {
                case State.Detect:
                    if (IsFacingTarget())
                    {
                        m_physics.velocity = Vector2.zero;
                        m_stateHandle.Wait(State.Chasing);
                        StartCoroutine(DetectRoutine());
                    }
                    else
                    {
                        CustomTurn();
                    }
                    break;

                case State.Idle:
                    m_animation.SetAnimation(0, m_idleAnimation, true);
                    break;

                case State.Attacking:
                    m_stateHandle.Wait(State.Cooldown);

                    m_rootMotion.transformPositionX = false;
                    var attack = Attack.Attack;
                    switch (attack)
                    {
                        case Attack.Attack:
                            m_attackRoutine = StartCoroutine(AttackRoutine());
                            break;
                    }

                    break;

                case State.Cooldown:
                    //m_stateHandle.Wait(State.ReevaluateSituation);
                    if (!IsFacingTarget())
                    {
                        CustomTurn();
                    }
                    else
                    {
                        m_animation.SetAnimation(0, m_idleAnimation, true);
                    }

                    if (m_currentCD <= m_currentFullCD)
                    {
                        m_currentCD += Time.deltaTime;
                    }
                    else
                    {
                        m_currentCD = 0;
                        m_stateHandle.OverrideState(State.Chasing);
                    }

                    break;

                case State.Chasing:
                    {
                        Debug.Log("Facing Target " + IsFacingTarget());
                        if (IsFacingTarget())
                        {
                            if (IsTargetInRange(m_attackRange))
                            {
                                m_physics.velocity = Vector2.zero;
                                m_animation.SetAnimation(0, m_idleAnimation, true);
                                m_stateHandle.SetState(State.Attacking);
                            }
                            else
                            {
                                m_rootMotion.transformPositionX = true;
                                if (m_state.isGrounded)
                                {
                                    m_animation.SetAnimation(0, m_moveAnimation, true);
                                    //m_physics.velocity = new Vector2(20f, m_physics.velocity.y);
                                }
                                else
                                {
                                    m_physics.velocity = Vector2.zero;
                                    m_animation.SetAnimation(0, m_idleAnimation, true);
                                }
                            }
                        }
                        else
                        {
                            CustomTurn();
                        }
                    }
                    break;
                case State.WaitBehaviourEnd:
                    return;
            }
        }
    }
}
