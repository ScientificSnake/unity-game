using UnityEngine;

public class RocketBehavior : ProjectileTemplate
{
    public float Fuel;//time the rocket accelerates for
    public float Acceleration;//rate at which the rocket accelerates
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    public float Damage;

    public HealthScript rocketHealth;

    protected override void Awake()
    {
        base.Awake();
        rocketHealth = gameObject.GetComponent<HealthScript>();
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if ( tcollider == null )
            tcollider = GetComponent<PolygonCollider2D>();
        if (Fuel > 0)   
        {
            Vector2 Direction = transform.right.normalized;

            rb.AddForce(Direction * Acceleration);
            Fuel--;
        }
        if (Fuel == 0)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGo = collision.gameObject;

        if (OtherGo.TryGetComponent<HealthScript>(out HealthScript otherHealth))
        {
            otherHealth.ApplyDamage(Damage);
            Destroy(gameObject);
        }
        else if (OtherGo.TryGetComponent(out BulletBehavior bullet))
        {
            // just take the damage and do not destr
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
