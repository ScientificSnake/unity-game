using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class BulletBehavior : ProjectileTemplate
{
    public float Damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGO = collision.gameObject;

        if (OtherGO.TryGetComponent<EnemyTemplate>(out EnemyTemplate targetScript))
        {
            targetScript.ApplyDamage(Damage);
        }

        if (OtherGO.CompareTag("Player"))
        {
            PlayerObjectScript playerObjectScript = OtherGO.GetComponent<PlayerObjectScript>();
            playerObjectScript.ApplyDamage(Damage);
        }
        // destroy self
        Destroy(gameObject);
    }
}