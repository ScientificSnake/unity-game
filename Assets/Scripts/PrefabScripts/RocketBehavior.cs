using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class RocketBehavior : ProjectileTemplate
{
    public float Fuel;//time the rocket accelerates for
    public float Acceleration;//rate at which the rocket accelerates
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    public float Damage;
    public static PlayerObjectScript PlayerScriptRef;

    public HealthScript rocketHealth;

    protected override void Awake()
    {
        if (PlayerScriptRef == null)
        {
            PlayerScriptRef = GameObject.FindWithTag("Player").GetComponent<PlayerObjectScript>();
        }

        base.Awake();
        rocketHealth = gameObject.GetComponent<HealthScript>();
        rocketHealth.Health = 220;
        contactFilter = new();
        contactFilter.useTriggers = true;  // Include triggers
        contactFilter.useLayerMask = false; // Check all layers
        DamageRadius = 50;

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

    private static List<Collider2D> colliderList = new List<Collider2D>(32);
    private ContactFilter2D contactFilter;
    private float DamageRadius;

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip boomBoomSound;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGo = collision.gameObject;

        if (OtherGo.TryGetComponent<HealthScript>(out HealthScript otherHealth))
        {
            otherHealth.ApplyDamage(Damage);
            PlayerScriptRef.audios.PlayOneShot(boomBoomSound, 0.2f);
            ObjTools.ApplyExplosionDamage(colliderList, gameObject, DamageRadius, contactFilter, Damage / 3, 50);
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            explosion.GetComponent<Rigidbody2D>().linearVelocity = rb.linearVelocity;
            explosion.transform.localScale = new Vector3(50, 50, 50);
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
