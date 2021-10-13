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
    public class ChibiArcherAI : CombatAIBrain
    {
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_idleAnimation;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_attackAnimation;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_deathAnimation;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_moveAnimation;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation"), TabGroup("Animation")]
        private string m_turnAnimation;

        [SerializeField, TabGroup("AttackStats")]
        private float m_attackRange;

        private enum State
        {
            Detect,
            Idle,
            Turning,
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

        [SerializeField, TabGroup("Reference")]
        private ProjectileLauncher m_launcher;
        [SerializeField, TabGroup("Modules")]
        private SkeletonRootMotion m_rootMotion;
        [SerializeField, TabGroup("Modules")]
        private AnimatedTurnHandle m_turnHandle;

        [SerializeField, TabGroup("Bone")]
        private SkeletonUtilityBone m_projectileBone;

        private float m_currentCD;
        private float m_currentFullCD;

        [ShowInInspector]
        private StateHandle<State> m_stateHandle;

        private State m_turnState;

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

        private void OnFlinchStart(object sender, EventActionArgs eventArgs)
        {
            if (m_attackRoutine != null)
            {
                StopCoroutine(m_attackRoutine);
                m_attackRoutine = null;
                DeactivateAim();
            }
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
            m_animation.SetAnimation(0, m_attackAnimation, false);
            yield return new WaitForAnimationComplete(m_animation.animationState, m_attackAnimation);
            m_stateHandle.ApplyQueuedState();
            yield return null;
        }

        private IEnumerator DeathRoutine()
        {
            m_animation.SetAnimation(0, m_deathAnimation, false);
            yield return new WaitForAnimationComplete(m_animation.animationState, m_deathAnimation);
            this.gameObject.SetActive(false);
            yield return null;
        }

        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            m_stateHandle.SetState(State.Detect);
        }

        public void ActivateAim()
        {
            Debug.Log("Activate AIM");
            m_projectileBone.mode = SkeletonUtilityBone.Mode.Override;
            //var targetPos = new Vector2(m_target.position.x, m_target.position.y + 10f);
            m_launcher.AimAt(new Vector2(m_target.position.x, m_target.position.y + 5f));
            m_projectileBone.transform.position = new Vector2(m_target.position.x, m_target.position.y + 10f);
        }

        public void DeactivateAim()
        {
            m_projectileBone.mode = SkeletonUtilityBone.Mode.Follow;
        }

        protected override void Start()
        {
            base.Start();
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
            m_flinch.gameObject.SetActive(false);
            enabled = false;
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
                        m_turnState = State.Detect;
                        if (m_animation.GetCurrentAnimation(0).ToString() != m_turnAnimation)
                            m_stateHandle.SetState(State.Turning);
                    }
                    break;

                case State.Idle:
                    m_animation.SetAnimation(0, m_idleAnimation, true);
                    break;

                case State.Turning:
                    m_stateHandle.Wait(m_turnState);
                    m_turnHandle.Execute(m_turnAnimation, m_idleAnimation);
                    break;

                case State.Attacking:
                    m_stateHandle.Wait(State.Cooldown);

                    m_rootMotion.transformPositionX = true;
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
                        m_turnState = State.Cooldown;
                        if (m_animation.GetCurrentAnimation(0).ToString() != m_turnAnimation)
                            m_stateHandle.SetState(State.Turning);
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
                            m_rootMotion.transformPositionX = true;
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
                                    //m_movement.MoveTowards(Vector2.one * transform.localScale.x, m_info.move.speed);
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
                            m_turnState = State.Chasing;
                            if (m_animation.GetCurrentAnimation(0).ToString() != m_turnAnimation)
                                m_stateHandle.SetState(State.Turning);
                        }
                    }
                    break;
                case State.WaitBehaviourEnd:
                    return;
            }
        }
    }
}
