using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDeactivationZone : MonoBehaviour
{
    private Collider2D HitBox;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<HitboxHolder>() != null && collision.GetComponent<Collider2D>().gameObject.name != "AggroSensor")
        {
            HitBox = collision.GetComponentInParent<HitboxHolder>().SendingHitBox;
            HitBox.enabled = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<HitboxHolder>() != null && collision.GetComponent<Collider2D>().gameObject.name != "AggroSensor")
        {
            HitBox = collision.GetComponentInParent<HitboxHolder>().SendingHitBox;
            HitBox.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<HitboxHolder>() != null && collision.GetComponent<Collider2D>().gameObject.name != "AggroSensor")
        {
            HitBox = collision.GetComponentInParent<HitboxHolder>().SendingHitBox;
            HitBox.enabled = false;
        }
    }
}
