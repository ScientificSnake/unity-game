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

    public float ExplosionDamage;
    public float DamageRadius;
    public float BlastForce;
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

        BlastForce = 100;
        DamageRadius = 100;
        ExplosionDamage = 400;

        Audio = gameObject.AddComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGo = collision.gameObject;
        if (OtherGo.CompareTag("Player"))
        {
            if ((collision.relativeVelocity.magnitude > 50) && State != "Detonating")
            {
                Throttle = 0;
                State = "Detonating";
                Audio.PlayOneShot(BoomBoomSound, 0.2f);

                ManagerScript.Instance.RunOnDelay(Explode, 2);
                //StartCoroutine(RunOnDelayCR(Explode, 2));
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
        ManagerScript.Instance.RunOnDelay(DestroyExplosionObj, 1);
        //StartCoroutine(RunOnDelayCR(DestroyExplosionObj, 1));
        void DestroyKamikaz()
        {
            try
            {
                Destroy(gameObject);
            }
            catch
            {
                print("Kamkaz already killed self");
            }
        }
        ManagerScript.Instance.RunOnDelay(DestroyKamikaz, 1);
        //StartCoroutine(RunOnDelayCR(DestroyKamikaz, 1));

        #region Damage application and physics

        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, DamageRadius);

        foreach (Collider2D collider in collidersInRadius)
        {
            float distanceToSelf = Vector2.Distance(collider.gameObject.transform.position, transform.position);
            float intensityProportion = (DamageRadius - distanceToSelf)/DamageRadius;

            GameObject OtherGo = collider.gameObject;

            if (OtherGo.TryGetComponent<PlayerObjectScript>(out PlayerObjectScript _))
            {
                // already have a reference
                PlayerScriptRef.ApplyDamage(intensityProportion * ExplosionDamage);
            }
            else if(OtherGo.TryGetComponent<EnemyTemplate>(out EnemyTemplate enemyScript))
            {
                enemyScript.ApplyDamage(intensityProportion * ExplosionDamage);
            }

            Vector2 directionToCollider = OtherGo.transform.position - collider.transform.position;

            if (OtherGo.TryGetComponent<Rigidbody2D>(out Rigidbody2D OtherRb))
            {
                float force = intensityProportion * BlastForce;
                OtherRb.AddForce(directionToCollider.normalized * force);
            }
        }


        #endregion
    }
}