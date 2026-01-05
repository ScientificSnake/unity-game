using System.Collections.Generic;
using System;
using UnityEngine;

public class BulletBehavior : ProjectileTemplate
{
    public float Damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGo = collision.gameObject;

        if (OtherGo.TryGetComponent<HealthScript>(out HealthScript otherHealth))
        {
            otherHealth.ApplyDamage(Damage);
            ApplyHitFuncs(collision);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}