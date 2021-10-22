using ChibiKnight.Gameplay;
using ChibiKnight.Systems.Combat;
using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputTranslator m_input;
    [SerializeField]
    private CharacterState m_state;
    [SerializeField]
    private float m_movementSpeed;
    [SerializeField, MinValue(0.1f)]
    private float m_jumpPower;
    [SerializeField, MinValue(0.1f)]
    private float m_highJumpPower;
    [SerializeField]
    private LayerMask m_collisionLayer;
    [SerializeField]
    private PhysicsMaterial2D m_groundedPhysicsMaterial;
    [SerializeField]
    private PhysicsMaterial2D m_midairPhysicsMaterial;
    [SerializeField]
    private SkeletonAnimationHelper m_animator;

    //Ground Check
    [SerializeField]
    private Vector2 m_origin;
    [SerializeField]
    private Vector2 boxSize;
    [SerializeField]
    private float angle;
    [SerializeField]
    private float m_groundCheckOffset;
    private bool m_previouslyAirborne = false;

    //Jump
    [SerializeField]
    private float m_highJumpDuration;
    [SerializeField]
    private bool m_isFalling = false;

    //Attacks
    [SerializeField]
    private float m_comboCooldown;
    [SerializeField]
    private List<AttackInfo> m_attackColliders;
    private bool m_canAttack = true;
    private int m_currentCombo = 1;

    //Death
    [SerializeField]
    private Collider2D m_hitbox;
    [SerializeField]
    private float m_deathX;
    [SerializeField]
    private float m_deathY;
    [SerializeField]
    private float m_damagedInvulnerableTimer;
    [SerializeField]
    private float m_deathTimer;
    private float m_currentDamagedInvulnerableTimer;
    private float m_currentDeathTimer;

    private Rigidbody2D m_rigidbody;
    private ContactFilter2D m_filter;
    private List<Collider2D> m_colliderList;
    private float m_highJumpCurrentTimer;
    private bool m_canHighJump = true;
    private float m_originalMoveSpeed;
    private float m_comboCooldownTimer;
    private Damageable m_damageable;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (m_input.walkPressed)
        {
            m_state.waitForBehaviour = false;
        }

        if (m_state.isInvulnerable)
        {
            m_currentDamagedInvulnerableTimer -= Time.deltaTime;

            if (m_animator.skeletonAnimation.Skeleton.Skin.ToString() == "default")
            {
                m_animator.skeletonAnimation.Skeleton.SetSkin("ChibiDeath");
            }
            else
            {
                m_animator.skeletonAnimation.Skeleton.SetSkin("default");
            }

            if (m_currentDamagedInvulnerableTimer <= 0)
            {
                m_state.isInvulnerable = false;
                m_hitbox.enabled = true;
                m_currentDamagedInvulnerableTimer = m_damagedInvulnerableTimer;
                m_animator.skeletonAnimation.Skeleton.SetSkin("ChibiDeath");
            }
        }

        if (m_state.isDead)
        {
            m_currentDeathTimer -= Time.deltaTime;

            if (m_currentDeathTimer <= 0)
            {
                gameObject.SetActive(false);
            }

            return;
        }

        if (m_state.isAttacking == false)
        {
            HandleAttackComboCooldown();
        }

        if (m_state.waitForBehaviour)
        {
            return;
        }

        EvaluateGroundedness();

        if (m_state.isGrounded)
        {
            HandleGroundBehaviour();
        }
        else
        {
            HandleAerialBehaviour();
        }
    }

    private void Initialize()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_damageable = GetComponent<Damageable>();

        //Ground Check Stuff
        m_filter = new ContactFilter2D();
        m_filter.useTriggers = false;
        m_filter.useLayerMask = true;
        m_filter.layerMask = m_collisionLayer;
        m_colliderList = new List<Collider2D>();

        m_highJumpCurrentTimer = m_highJumpDuration;
        m_originalMoveSpeed = m_movementSpeed;
        m_comboCooldownTimer = m_comboCooldown;

        m_damageable.OnDeath2 += HandleDeath;
        m_currentDeathTimer = m_deathTimer;
        m_currentDamagedInvulnerableTimer = m_damagedInvulnerableTimer;
    }

    private void HandleGroundBehaviour()
    {
        if (m_state.isAttacking)
        {
            return;
        }

        if (m_input.walkPressed)
        {
            m_movementSpeed /= 2;
            m_state.isWalking = true;
        }
        else if (m_input.walkHeld == false)
        {
            m_movementSpeed = m_originalMoveSpeed;
            m_state.isWalking = false;
        }

        if (m_input.jumpPressed)
        {
            HandleJump();
        }
        else
        {
            HandleMovement(m_input.horizontalInput);
        }

        if (m_canAttack == true)
        {
            if (m_input.slashPressed)
            {
                Debug.Log("Slash Pressed");
                HandleAttack();
            }
            else if (m_input.ultimateSlashPressed)
            {
                HandleUltimateSlash();
            }
        }
    }

    private void HandleAerialBehaviour()
    {
        if (m_state.isHighJumping)
        {
            if (m_input.jumpHeld && m_canHighJump == true)
            {
                HandleHighJump();
            }
            else
            {
                m_state.isHighJumping = false;
            }
        }

        HandleMovement(m_input.horizontalInput);
    }

    private void HandleMovement(float direction)
    {
        if (direction == 0 && m_state.isGrounded)
        {
            if (m_state.isGrounded)
            {
                m_animator.SetAnimation(0, "Idle1", true);
            }
        }
        else
        {
            if (m_state.isGrounded)
            {
                if (m_state.isWalking == true)
                {
                    m_animator.SetAnimation(0, "Walk_inPlace", true);
                }
                else
                {
                    m_animator.SetAnimation(0, "Run_inPlace", true);
                }
            }
            else
            {
                if (m_rigidbody.velocity.y > 0)
                {
                    m_animator.SetAnimation(0, "Jump_1Rise_Loop", true, 0);
                }
                else if (m_rigidbody.velocity.y < 0)
                {
                    if (m_isFalling == false)
                    {
                        m_isFalling = true;
                        m_animator.SetAnimation(0, "Jump_2RisetoFalling", false, 0);
                        m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, -10f);
                        m_animator.skeletonAnimation.state.Complete += JumpRiseToFallingState_Complete;
                    }
                    else
                    {
                        //m_animator.SetAnimation(0, "Jump_3Fall_Loop", true, 0);
                    }
                }
            }
        }

        var xVelocity = m_movementSpeed * direction;
        m_rigidbody.velocity = new Vector2(xVelocity, m_rigidbody.velocity.y);

        if (direction != 0)
        {
            HandleFacing(direction);
        }
    }

    private void HandleJump()
    {
        m_state.isHighJumping = true;
        m_animator.SetAnimation(0, "Jump_1Rise_Loop", false, 0);
        m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_jumpPower);
        m_highJumpCurrentTimer = m_highJumpDuration;

        //Reset combo
        m_currentCombo = 1;
        m_comboCooldownTimer = m_comboCooldown;
    }

    private void HandleHighJump()
    {
        //m_animator.SetAnimation(0, "Jump_1Rise", false, 0);
        m_highJumpCurrentTimer -= Time.deltaTime;

        if (m_highJumpCurrentTimer <= 0)
        {
            m_canHighJump = false;
            m_state.isHighJumping = false;
        }
        else
        {
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_highJumpPower);
        }
    }

    private void HandleAttack()
    {
        m_state.isAttacking = true;
        m_state.waitForBehaviour = true;

        m_rigidbody.velocity = Vector2.zero;

        if (m_currentCombo == 1)
        {
            m_animator.SetAnimation(0, "Attack1", false, 0);
            m_attackColliders[m_currentCombo - 1].m_collider.enabled = true;
        }
        else if (m_currentCombo == 2)
        {
            m_animator.SetAnimation(0, "Attack2", false, 0);
            m_attackColliders[m_currentCombo - 1].m_collider.enabled = true;
        }
        else if (m_currentCombo == 3)
        {
            m_animator.SetAnimation(0, "Attack3", false, 0);
            m_attackColliders[m_currentCombo - 1].m_collider.enabled = true;
        }

        m_currentCombo++;

        if (m_currentCombo == 4)
        {
            m_currentCombo = 1;
        }

        m_comboCooldownTimer = m_comboCooldown;
        m_rigidbody.AddForce(new Vector2(transform.localScale.x * m_attackColliders[m_currentCombo - 1].m_attackForce, 0), ForceMode2D.Force);
        m_animator.skeletonAnimation.state.Complete += AttackState_Complete;
    }

    private void HandleAttackComboCooldown()
    {
        m_comboCooldownTimer -= Time.deltaTime;

        if (m_comboCooldownTimer <= 0)
        {
            m_currentCombo = 1;
            m_comboCooldownTimer = m_comboCooldown;
        }
    }

    private void HandleUltimateSlash()
    {
        m_state.isAttacking = true;
        m_state.waitForBehaviour = true;

        m_animator.SetAnimation(0, "Attack1", false, 0);
        m_attackColliders[0].m_collider.enabled = true;

        m_animator.skeletonAnimation.state.Complete += UltimateAttackState_Complete;

        m_animator.SetAnimation(0, "Attack2", false, 0);
        m_attackColliders[1].m_collider.enabled = true;

        m_animator.skeletonAnimation.state.Complete += UltimateAttackState_Complete;

        m_animator.SetAnimation(0, "Attack3", false, 0);
        m_attackColliders[1].m_collider.enabled = true;

        m_animator.skeletonAnimation.state.Complete += UltimateAttackEndState_Complete;
    }

    private void HandleFacing(float direction)
    {
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.y);
    }

    private void HandleIdle()
    {
        m_animator.SetAnimation(0, "Idle1", true);
        m_rigidbody.velocity = Vector2.zero;
    }

    private void HandleDeath(Vector3 sourcePosition)
    {
        var direction = 0;
        Vector2 knockBackDirection = Vector2.zero;

        if (sourcePosition.x > transform.position.x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        knockBackDirection.x = direction;
        knockBackDirection.y = 1;

        m_state.waitForBehaviour = true;
        m_state.isDead = true;

        m_rigidbody.gravityScale = 10;
        m_rigidbody.drag = 0;

        m_rigidbody.velocity = Vector2.zero;
        m_rigidbody.AddForce(new Vector2(knockBackDirection.x * m_deathX, knockBackDirection.y * m_deathY), ForceMode2D.Impulse);
        m_animator.SetAnimation(0, "Flinch1", true);
        m_animator.skeletonAnimation.state.Complete += FlinchState_Complete;
    }

    private void EvaluateGroundedness()
    {
        int groundColliderResult = Physics2D.OverlapBox(m_origin + (Vector2)transform.position, boxSize, angle, m_filter, m_colliderList);
        var isGrounded = groundColliderResult > 0 ? true : false;

        if (m_state.isGrounded == false)
        {
            //if (isGrounded == true)
            //{
            //    m_state.waitForBehaviour = true;

            //    if (m_rigidbody.velocity.y <= -50f)
            //    {
            //        m_animator.SetAnimation(0, "Jump_4Land", false);
            //    }
            //    else
            //    {
            //        m_animator.SetAnimation(0, "Jump_4LandHard", false);
            //    }

            //    m_rigidbody.velocity = Vector2.zero;
            //    m_animator.skeletonAnimation.state.Complete += LandState_Complete;
            //}
        }

        m_state.isGrounded = isGrounded;

        if (isGrounded == true)
        {
            m_rigidbody.sharedMaterial = m_groundedPhysicsMaterial;
            m_canHighJump = true;
            m_isFalling = false;
        }
        else
        {
            m_rigidbody.sharedMaterial = m_midairPhysicsMaterial;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(m_origin + (Vector2)transform.position, boxSize);
    }

    private void FlinchState_Complete(Spine.TrackEntry trackEntry)
    {

    }

    private void AttackState_Complete(Spine.TrackEntry trackEntry)
    {
        m_state.isAttacking = false;
        m_state.waitForBehaviour = false;

        m_animator.skeletonAnimation.state.Complete -= AttackState_Complete;

        for (int i = 0; i < m_attackColliders.Count; i++)
        {
            m_attackColliders[i].m_collider.enabled = false;
        }
    }

    private void JumpRiseToFallingState_Complete(Spine.TrackEntry trackEntry)
    {
        m_animator.skeletonAnimation.state.Complete -= JumpRiseToFallingState_Complete;
        m_animator.AddAnimation(0, "Jump_3Fall_Loop", true, 0);
    }

    private void LandState_Complete(Spine.TrackEntry trackEntry)
    {
        m_animator.skeletonAnimation.state.Complete -= LandState_Complete;
        m_state.waitForBehaviour = false;
    }

    private void UltimateAttackState_Complete(Spine.TrackEntry trackEntry)
    {
        m_animator.skeletonAnimation.state.Complete -= AttackState_Complete;

        for (int i = 0; i < m_attackColliders.Count; i++)
        {
            m_attackColliders[i].m_collider.enabled = false;
        }
    }

    private void UltimateAttackEndState_Complete(Spine.TrackEntry trackEntry)
    {
        m_animator.skeletonAnimation.state.Complete -= UltimateAttackEndState_Complete;

        for (int i = 0; i < m_attackColliders.Count; i++)
        {
            m_attackColliders[i].m_collider.enabled = false;
        }

        m_state.isAttacking = false;
        m_state.waitForBehaviour = false;
    }
}
