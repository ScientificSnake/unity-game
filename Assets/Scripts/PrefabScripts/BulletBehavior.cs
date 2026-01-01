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
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}