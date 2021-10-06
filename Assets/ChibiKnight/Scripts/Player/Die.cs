using ChibiKnight.Systems.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    private Damageable m_damageable;

    // Start is called before the first frame update
    void Start()
    {
        m_damageable = GetComponentInParent<Damageable>();

        if (m_damageable != null)
        {
            m_damageable.OnDeath += OnDeath;
        }
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }
}
