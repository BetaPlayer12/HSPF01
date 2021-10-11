using ChibiKnight.Gameplay;
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
    private bool m_isFalling = false;

    //Attacks
    [SerializeField]
    private List<Collider2D> m_attackColliders;
    private bool m_canAttack = true;
    private int m_currentCombo = 1;

    private Rigidbody2D m_rigidbody;
    private ContactFilter2D m_filter;
    private List<Collider2D> m_colliderList;
    private float m_highJumpCurrentTimer;
    private bool m_canHighJump = true;
    private float m_originalMoveSpeed;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        EvaluateGroundedness();

        if (m_state.waitForBehaviour)
        {
            return;
        }

        if (m_state.isGrounded)
        {
            HandleGroundBehaviour();
        }
        else
        {
            HandleAerialBehaviour();
        }
    }

    private void FixedUpdate()
    {

    }

    private void Initialize()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();

        //Ground Check Stuff
        m_filter = new ContactFilter2D();
        m_filter.useTriggers = false;
        m_filter.useLayerMask = true;
        m_filter.layerMask = m_collisionLayer;
        m_colliderList = new List<Collider2D>();

        m_highJumpCurrentTimer = m_highJumpDuration;
        m_originalMoveSpeed = m_movementSpeed;
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
        //else if (m_input.horizontalInput == 0)
        //{
        //    HandleIdle();
        //}
        else
        {
            HandleMovement(m_input.horizontalInput);
        }

        if (m_canAttack == true)
        {
            if (m_input.slashPressed)
            {
                Debug.Log("Slash Pressed");
                PrepareAttack();
                HandleAttack();
            }
        }
    }

    private void PrepareAttack()
    {

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
                        m_animator.skeletonAnimation.state.Complete += JumpRiseToFallingState_Complete;
                    }
                    //else
                    //{
                    //    m_animator.SetAnimation(0, "Jump_3Fall_Loop", true, 0);
                    //}
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

        if (m_currentCombo == 1)
        {
            m_animator.SetAnimation(0, "Attack1", false, 0);
            m_attackColliders[m_currentCombo - 1].enabled = true;
        }
        else if (m_currentCombo == 2)
        {
            m_animator.SetAnimation(0, "Attack2", false, 0);
            m_attackColliders[m_currentCombo - 1].enabled = true;
        }
        else if (m_currentCombo == 3)
        {
            m_animator.SetAnimation(0, "Attack3", false, 0);
            m_attackColliders[m_currentCombo - 1].enabled = true;
        }

        m_currentCombo++;

        if (m_currentCombo == 4)
        {
            m_currentCombo = 1;
        }

        m_animator.skeletonAnimation.state.Complete += State_Complete;
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

    private void EvaluateGroundedness()
    {
        int groundColliderResult = Physics2D.OverlapBox(m_origin + (Vector2)transform.position, boxSize, angle, m_filter, m_colliderList);
        var isGrounded = groundColliderResult > 0 ? true : false;

        if (m_state.isGrounded == false)
        {
            if (isGrounded == true)
            {
                Debug.Log("LAND");
                m_animator.SetAnimation(0, "Jump_4Land", false);
            }
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

    private void State_Complete(Spine.TrackEntry trackEntry)
    {
        m_state.isAttacking = false;
        m_animator.skeletonAnimation.state.Complete -= State_Complete;

        for (int i = 0; i < m_attackColliders.Count; i++)
        {
            m_attackColliders[i].enabled = false;
        }
    }

    private void JumpRiseToFallingState_Complete(Spine.TrackEntry trackEntry)
    {
        m_animator.skeletonAnimation.state.Complete -= State_Complete;
        m_animator.SetAnimation(0, "Jump_3Fall_Loop", true, 0);
    }
}
