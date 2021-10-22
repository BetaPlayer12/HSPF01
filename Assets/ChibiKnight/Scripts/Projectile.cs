using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private GameObject m_impactFX;
    [SerializeField]
    private LayerMask m_layerMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MainCamera"))
        {

        }
        else
        {
            var impactFX = Instantiate(m_impactFX, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
