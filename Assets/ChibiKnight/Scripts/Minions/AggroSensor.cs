using DChild.Gameplay.Characters.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroSensor : MonoBehaviour
{
    private CombatAIBrain m_aiBrain;

    private void Awake()
    {
        m_aiBrain = GetComponentInParent<CombatAIBrain>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerController>() != null)
        {
            m_aiBrain.SetTarget(collision.transform);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
