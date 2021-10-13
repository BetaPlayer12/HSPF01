using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxHolder : MonoBehaviour
{
    [SerializeField]
    private Collider2D DamageableHitBox;
    public Collider2D SendingHitBox => DamageableHitBox;
}
