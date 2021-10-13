using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;
using ChibiKnight.Gameplay;
using Holysoft.Event;
using System.Collections.Generic;
using ChibiKnight.Systems.Combat;

namespace DChild.Gameplay.Characters
{
    public class FlinchHolder : MonoBehaviour
    {
        [SerializeField]
        private SkeletonAnimationHelper m_animator;
        [SerializeField]
        private Spine.AnimationState m_animationState;
        [SerializeField]
        private Rigidbody2D m_physics;
        [SerializeField]
        private Damageable m_damageable;
        [SerializeField]
        private Transform m_centerMass;
        [SerializeField]
        private GameObject m_hitFX;
        [SerializeField]
        public float m_flinchCooldown;

#if UNITY_EDITOR
        [SerializeField]
        private SkeletonAnimation m_skeletonAnimation;
#endif
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation")]
        private List<string> m_flinchAnimations;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation")]
        private string m_flinchFXAnimation;
        [SerializeField, Spine.Unity.SpineAnimation(dataField = "m_skeletonAnimation")]
        private string m_idleAnimation;

        private string m_currentFlinchAnimation;

        private bool m_isFlinching;
        private bool m_canFlinch = true;

        private void Awake() => m_damageable.OnTakeDamage += Flinch;

        //public event EventAction<EventActionArgs> HitStopStart;
        public event EventAction<EventActionArgs> FlinchStart;
        public event EventAction<EventActionArgs> FlinchEnd;
        public bool isFlinching => m_isFlinching;

        private Coroutine m_flinchRoutine;

        public void CanFlinch(bool allowFlinch)
        {
            m_canFlinch = allowFlinch;
        }

        private string RandomFlinch()
        {
            int flinch = 0;
            if (m_flinchAnimations.Count > 1)
            {
                while (m_flinchAnimations[flinch] == m_currentFlinchAnimation)
                {
                    flinch = UnityEngine.Random.Range(0, m_flinchAnimations.Count);
                }
            }
            return m_flinchAnimations[flinch];
        }

        public virtual void Flinch(Vector3 directionToSource)
        {
            m_animator.SetEmptyAnimation(1, 0);
            Instantiate(m_hitFX, m_centerMass.position, Quaternion.identity);
            m_animator.SetAnimation(1, m_flinchFXAnimation, false, 0).MixDuration = 0;
            if (m_canFlinch)
            {
                Flinch();
            }
        }

        public void Flinch()
        {
            if (m_isFlinching == false)
            {
                StartFlinch();
            }
        }

        private void UpdateFlinchRestrictions()
        {

        }

        private void StartFlinch()
        {
            m_physics.velocity = Vector2.zero;
            m_flinchRoutine = StartCoroutine(FlinchRoutine());
        }

        private IEnumerator FlinchRoutine()
        {
            FlinchStart?.Invoke(this, new EventActionArgs());
            m_currentFlinchAnimation = RandomFlinch();
            m_animator.SetAnimation(0, m_currentFlinchAnimation, false, 0).MixDuration = 0;
            m_animator.AddAnimation(0, m_idleAnimation, true, 0);

            //m_spine.AddEmptyAnimation(0, 0.2f, 0);
            m_isFlinching = true;
            yield return new WaitForAnimationComplete(m_animationState, m_idleAnimation);
            //m_animator.SetAnimation(0, m_idleAnimation, true);
            yield return new WaitForSeconds(m_flinchCooldown);
            m_isFlinching = false;
            FlinchEnd?.Invoke(this, new EventActionArgs());
        }
    }
}