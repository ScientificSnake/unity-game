using UnityEngine;

public class KamikazeEnemyAI : EnemyTemplate
{
    float SpriteAngleOffset = 180;
    protected void RotateTowardsPlayer()
    {
        Vector2 vectorToPlayer = (Vector2)(PlayerRef.transform.position - transform.position);
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg + SpriteAngleOffset;

        // For 2D, rotate around Z-axis
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    protected void FixedUpdate()
    {
        RotateTowardsPlayer();
    }
}