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

        private bool m_isFlinching;

        private void Awake() => m_damageable.OnTakeDamage += Flinch;

        //public event EventAction<EventActionArgs> HitStopStart;
        public event EventAction<EventActionArgs> FlinchStart;
        public event EventAction<EventActionArgs> FlinchEnd;
        public bool isFlinching => m_isFlinching;

        private Coroutine m_flinchRoutine;


        private string RandomFlinch()
        {
            int flinch = UnityEngine.Random.Range(0, m_flinchAnimations.Count);
            return m_flinchAnimations[flinch];
        }

        public virtual void Flinch(Vector3 directionToSource)
        {

            Flinch();
        }

        public void Flinch()
        {
            m_animator.SetEmptyAnimation(1, 0);
            if (m_isFlinching == false)
            {
                StartFlinch();
            }
            else
            {
                m_animator.SetAnimation(1, m_flinchFXAnimation, false, 0).MixDuration = 0;
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
            var flinchAnim = RandomFlinch();
            m_animator.SetAnimation(0, flinchAnim, false, 0).MixDuration = 0;

            //m_spine.AddEmptyAnimation(0, 0.2f, 0);
            m_isFlinching = true;
            yield return new WaitForAnimationComplete(m_animationState, flinchAnim);
            m_animator.SetAnimation(0, m_idleAnimation, true);
            yield return new WaitForSeconds(m_flinchCooldown);
            m_isFlinching = false;
            FlinchEnd?.Invoke(this, new EventActionArgs());
        }
    }
}