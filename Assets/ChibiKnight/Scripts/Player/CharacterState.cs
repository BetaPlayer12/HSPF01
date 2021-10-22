using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private bool m_isGrounded;
    public bool isGrounded { get => m_isGrounded; set => m_isGrounded = value; }
    [SerializeField, ReadOnly]
    private bool m_canAttack;
    public bool canAttack { get => m_canAttack; set => m_canAttack = value; }
    [SerializeField, ReadOnly]
    private bool m_isAttacking;
    public bool isAttacking { get => m_isAttacking; set => m_isAttacking = value; }
    [SerializeField, ReadOnly]
    private bool m_isHighJumping;
    public bool isHighJumping { get => m_isHighJumping; set => m_isHighJumping = value; }
    [SerializeField, ReadOnly]
    private bool m_isWalking;
    public bool isWalking { get => m_isWalking; set => m_isWalking = value; }
    [SerializeField, ReadOnly]
    private bool m_waitForBehaviour;
    public bool waitForBehaviour { get => m_waitForBehaviour; set => m_waitForBehaviour = value; }
    [SerializeField, ReadOnly]
    private bool m_isDead;
    public bool isDead { get => m_isDead; set => m_isDead = value; }
    [SerializeField, ReadOnly]
    private bool m_isInvulnerable;
    public bool isInvulnerable { get => m_isInvulnerable; set => m_isInvulnerable = value; }
}
