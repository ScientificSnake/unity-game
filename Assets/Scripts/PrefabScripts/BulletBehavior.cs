using UnityEngine;

public class BulletBehavior : ProjectileTemplate
{
    public float Damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGO = collision.gameObject;

        if (OtherGO.TryGetComponent<EnemyTemplate>(out EnemyTemplate targetScript))
        {
            targetScript.ApplyDamage(Damage);
            targetScript.SpawnSparks();
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