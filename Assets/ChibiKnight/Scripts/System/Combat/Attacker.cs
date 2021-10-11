using Sirenix.OdinInspector;
using UnityEngine;

namespace ChibiKnight.Systems.Combat
{
    public class Attacker : MonoBehaviour
    {
        [SerializeField, MinValue(1)]
        private int m_damage = 1;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Hitbox") == false)
                return;

            collision.GetComponentInParent<Damageable>().TakeDamage(m_damage, gameObject.transform.position);
        }
    }
}