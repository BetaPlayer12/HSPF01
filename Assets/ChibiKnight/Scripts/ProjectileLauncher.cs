using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField, TabGroup("Projectile")]
    private GameObject m_projectile;
    [SerializeField, TabGroup("Projectile")]
    private float m_projectileSpeed;
    [SerializeField]
    private Transform m_spawnPoint;

    public void AimAt(Vector2 target)
    {
        Vector2 v_diff = (target - (Vector2)m_spawnPoint.position);
        float atan2 = Mathf.Atan2(v_diff.y, v_diff.x);
        m_spawnPoint.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg);
    }

    public void LaunchProjectile()
    {
        var spawnDirection = m_spawnPoint.right;
        var projectile = Instantiate(m_projectile, m_spawnPoint.position, m_spawnPoint.rotation);
        projectile.GetComponent<Rigidbody2D>().velocity = spawnDirection * m_projectileSpeed;
    }
}
