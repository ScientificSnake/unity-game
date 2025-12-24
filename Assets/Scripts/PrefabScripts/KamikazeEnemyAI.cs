using MathNet.Numerics;
using System.Collections;
using System.Security.Authentication.ExtendedProtection;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering;

public class KamikazeEnemyAI : EnemyTemplate
{
    public float PlayerDetectionRadius;
    public string State = "LockedPlayer";
    public float SpriteAngleOffset = 0;
    public AudioClip BoomBoomSound;
    public AudioSource Audio;

    public float DamageRadius;
    protected void RotateTowardsPlayer()
    {
        Vector2 vectorToPlayer = (Vector2)(PlayerRef.transform.position - transform.position);
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg + SpriteAngleOffset;

        // For 2D, rotate around Z-axis
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    protected void UpdateState()
    {
        if (State != "Detonating")
        {
            if ((Vector2.Distance(transform.position, PlayerRef.transform.position) < PlayerDetectionRadius) && (ObjTools.LineOfSight(gameObject, PlayerRef.transform, PlayerDetectionRadius)))
            {
                State = "LockedPlayer";
            }
            else
            {
                State = "Wandering";
            }
        }
    }

    protected void FixedUpdate()
    {
        UpdateState();
        if (State == "LockedPlayer")
        {
            RotateTowardsPlayer();
            Throttle = 50;
        }
        else
        {
            Throttle = 0;
        }
        ObjTools.ApplyThrottle(Throttle, ref Fuel, MaxAccel, rb, transform, FuelUsage);
    }

    protected void Awake()
    {
        Fuel = 50 / Time.fixedDeltaTime;
        FuelUsage = 1;
        MaxAccel = 200;
        PlayerDetectionRadius = 2000;

        Audio = gameObject.AddComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGo = collision.gameObject;
        if (OtherGo.CompareTag("Player"))
        {
            if ((collision.relativeVelocity.magnitude > 20) && State != "Detonating")
            {
                Throttle = 0;
                State = "Detonating";
                Audio.PlayOneShot(BoomBoomSound, 0.2f);
                StartCoroutine(RunOnDelayCR(Explode, 2));
            }
        }
    }

    protected void Explode()
    {
        GameObject explosion = Instantiate(ManagerScript.Instance.ExplosionSystem);
        explosion.transform.localScale = new Vector3(150, 150, 150);
        explosion.transform.position = transform.position;
        Rigidbody2D explosionrb = explosion.GetComponent<Rigidbody2D>();
        explosionrb.linearVelocity = rb.linearVelocity;

        void DestroyExplosionObj()
        {
            Destroy(explosion);
        }

        Color color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;

        StartCoroutine(RunOnDelayCR(DestroyExplosionObj, 1));
        void DestroyKamikaz()
        {
            Destroy(gameObject);
        }
        StartCoroutine(RunOnDelayCR(DestroyKamikaz, 1));
        
    }
}