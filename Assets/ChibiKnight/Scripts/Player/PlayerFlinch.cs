using ChibiKnight.Gameplay;
using ChibiKnight.Systems.Combat;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlinch : MonoBehaviour
{
    [SerializeField]
    private CharacterState m_state;
    [SerializeField]
    private Damageable m_damageable;
    [SerializeField]
    private int m_numberOfFlinchStates;
    [SerializeField, MinValue(0)]
    private float m_XknockBackPower;
    [SerializeField, MinValue(0)]
    private float m_YknockbackPower;
    [SerializeField]
    private float m_aerialKnockBackMultiplier;
    [SerializeField]
    private float m_flinchDuration;
    [SerializeField]
    private float m_flinchGravityScale;
    [SerializeField]
    private GameObject m_hitFX;
    [SerializeField]
    private Transform m_hitFXSpawnPoint;
    [SerializeField]
    private Collider2D m_hitbox;

    private Rigidbody2D m_rigidBody;
    private SkeletonAnimationHelper m_animator;
    private float playerGravityScale;
    private float m_defaultLinearDrag;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        m_rigidBody = GetComponentInParent<Rigidbody2D>();
        m_animator = GetComponentInChildren<SkeletonAnimationHelper>();
        playerGravityScale = m_rigidBody.gravityScale;
        m_defaultLinearDrag = m_rigidBody.drag;
        m_damageable.OnTakeDamage += Flinch;
    }

    public void Flinch(Vector3 sourcePosition)
    {
        var direction = 0;
        Instantiate(m_hitFX, m_hitFXSpawnPoint.position, Quaternion.identity);

        if (sourcePosition.x > transform.position.x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        bool isAerialKnockback = false;
        m_rigidBody.velocity = Vector2.zero;
        Vector2 knockBackDirection = Vector2.zero;
        knockBackDirection.x = direction;

        if (m_state.isGrounded)
        {
            knockBackDirection.y = 1;
        }
        else
        {
            knockBackDirection.y = 1;
            isAerialKnockback = true;
        }

        StartCoroutine(FlinchRoutine(knockBackDirection, isAerialKnockback));
    }

    private IEnumerator FlinchRoutine(Vector2 direction, bool isAerialKnockBack)
    {
        float knockBackPower = m_XknockBackPower;
        float aerialKnockBackPower = m_YknockbackPower;
        int flinchState = Random.Range(1, m_numberOfFlinchStates + 1);

        m_state.waitForBehaviour = true;
        m_rigidBody.gravityScale = m_flinchGravityScale;
        m_rigidBody.drag = m_defaultLinearDrag;

        if (isAerialKnockBack == true)
        {
            aerialKnockBackPower = m_YknockbackPower * m_aerialKnockBackMultiplier;
        }

        m_rigidBody.velocity = Vector2.zero;
        m_rigidBody.AddForce(new Vector2(direction.x * knockBackPower, direction.y * aerialKnockBackPower), ForceMode2D.Impulse);
        m_animator.SetAnimation(0, "Flinch2", false);

        yield return new WaitForSeconds(m_flinchDuration);

        m_hitbox.enabled = false;
        m_state.isInvulnerable = true;
        m_state.waitForBehaviour = false;
        m_rigidBody.gravityScale = playerGravityScale;
    }
}
