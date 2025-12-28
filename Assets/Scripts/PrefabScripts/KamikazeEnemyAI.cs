using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemyAI : EnemyTemplate
{
    [Header("Detection")]
    public float PlayerDetectionRadius;

    public string State = "LockedPlayer";

    public float SpriteAngleOffset = 0;

    [Header("Audio")]
    public AudioClip BoomBoomSound;
    public AudioSource Audio;

    [Header("Weaponry")]
    public float FuseTime;
    public float ExplosionDamage;
    public float DamageRadius;
    public float BlastForce;

    [Header("Targeting")]
    public float ThrottleDegreeThreshold;
    public float ManualTargetingThreshold = 50;

    private ContactFilter2D contactFilter;
    private static List<Collider2D> colliderList = new List<Collider2D>(32);

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
            ObjTools.InterceptData interceptData = ObjTools.TryGetInterceptAngle(
                transform.position, rb.linearVelocity, PlayerRef.transform.position,
                PlayerScriptRef.rb.linearVelocity, rb.linearVelocity.magnitude);

            float targetAngle;

            //debug
            string angleMethod;

            if (interceptData.Possible && (PlayerScriptRef.rb.linearVelocity.magnitude > ManualTargetingThreshold))
            {
                angleMethod = "Intercept";
                targetAngle = interceptData.AimDeg;
            }
            else
            {
                angleMethod = "Manual";
                Vector2 toPlayerDir = PlayerRef.transform.position - transform.position;
                targetAngle = Mathf.Atan2(toPlayerDir.y, toPlayerDir.x) * Mathf.Rad2Deg;
            }

            RotateTowardsTargetAngle(targetAngle);
            //print($"<color=purple>{gameObject.name} is rotating towards angle {targetAngle}, method - {angleMethod}");
            if (IsAimedAtTarget(targetAngle, ThrottleDegreeThreshold))
            {
                Throttle = 100;
            }
            else
            {
                Throttle = 0;
            }
        }
        else
        {
            // helps get the lock back by moving
            Throttle = 10;
        }
        ObjTools.ApplyThrottle(Throttle, ref Fuel, MaxAccel, rb, transform, FuelUsage);
    }

    protected override void Awake()
    {
        base.Awake();

        Fuel = 50 / Time.fixedDeltaTime;
        FuelUsage = 1;
        MaxAccel = 200;
        PlayerDetectionRadius = 4000;

        rb.mass = 1;

        BlastForce = 100;
        DamageRadius = 150;
        ExplosionDamage = 600;
        FuseTime = 0.1f;
        ThrottleDegreeThreshold = 10;

        rotationDegreesPerSeconds = 135;

        Audio = gameObject.AddComponent<AudioSource>();

        contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;  // Include triggers
        contactFilter.useLayerMask = false; // Check all layers
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


                PlayerScriptRef.audios.PlayOneShot(BoomBoomSound, 0.2f); // this way after it dies the boom sound is still sounding
                ManagerScript.Instance.RunOnDelay(Explode, FuseTime);
                //StartCoroutine(RunOnDelayCR(Explode, 2));
            }
        }
    }

    protected void Explode()
    {
        MiniMapRegister.DeRegister(this);
        // VFX
        GameObject explosion = Instantiate(ManagerScript.Instance.ExplosionSystem);

        // Setup Explosion velo and position
        explosion.transform.localScale = new Vector3(150, 150, 150);
        explosion.transform.position = transform.position;
        Rigidbody2D explosionrb = explosion.GetComponent<Rigidbody2D>();
        explosionrb.linearVelocity = rb.linearVelocity;

        // Hide This kamikaze and disable collider
        Color color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;
        Collider2D tcollider = GetComponent<Collider2D>();
        tcollider.enabled = false;

        // Schedule clean up
        void DestroyExplosionObj()
        {
            Destroy(explosion);
        }
        void DestroyKamikaz()
        {
            try
            {
                Destroy(gameObject);
            }
            catch
            {
                print("Kamkaz already blew up self");
            }
        }
        ManagerScript.Instance.RunOnDelay(DestroyKamikaz, 1);
        ManagerScript.Instance.RunOnDelay(DestroyExplosionObj, 1);

        ApplyExplosionDamage();

        //#region Damage application and physics

        //colliderList.Clear();
        //int count = Physics2D.OverlapCircle(transform.position, DamageRadius, contactFilter, colliderList);

        //Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, DamageRadius);

        //foreach (Collider2D collider in collidersInRadius)
        //{
        //    float distanceToSelf = Mathf.Abs((Vector2.Distance(collider.gameObject.transform.position, transform.position)));
        //    float intensityProportion = (DamageRadius - distanceToSelf)/DamageRadius;

        //    GameObject OtherGo = collider.gameObject;

        //    if (OtherGo.TryGetComponent<PlayerObjectScript>(out PlayerObjectScript _))
        //    {
        //        // already have a reference
        //        PlayerScriptRef.ApplyDamage(intensityProportion * ExplosionDamage);
        //    }
        //    else if(OtherGo.TryGetComponent<EnemyTemplate>(out EnemyTemplate enemyScript))
        //    {
        //        enemyScript.ApplyDamage(intensityProportion * ExplosionDamage);
        //    }

        //    Vector2 directionToCollider = OtherGo.transform.position - transform.position;

        //    if (OtherGo.TryGetComponent<Rigidbody2D>(out Rigidbody2D OtherRb))
        //    {
        //        float force = intensityProportion * BlastForce;
        //        OtherRb.AddForce(directionToCollider.normalized * force);
        //    }
        //}
        //#endregion
    }

    protected void ApplyExplosionDamage()
    {
        // Clear list (but keep capacity for reuse)
        colliderList.Clear();

        // Use the modern List overload (not deprecated!)
        int hitCount = Physics2D.OverlapCircle(
            transform.position,
            DamageRadius,
            contactFilter,
            colliderList  // List automatically resizes if needed
        );

        // ========================================================================

        // Cache values to avoid repeated calculations
        Vector2 explosionPos = transform.position;
        float damageRadiusSqr = DamageRadius * DamageRadius;

        // Process all hit colliders
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D collider = colliderList[i];
            GameObject otherGo = collider.gameObject;

            // Skip self
            if (otherGo == gameObject) continue;

            // Calculate distance efficiently
            Vector2 toOther = (Vector2)otherGo.transform.position - explosionPos;
            float distanceSqr = toOther.sqrMagnitude;

            // Safety check
            if (distanceSqr > damageRadiusSqr) continue;

            // Calculate actual distance and intensity
            float distance = Mathf.Sqrt(distanceSqr);
            float intensityProportion = (DamageRadius - distance) / DamageRadius;

            // Apply damage
            if (otherGo.CompareTag("Player"))
            {
                PlayerScriptRef.ApplyDamage(intensityProportion * ExplosionDamage);
            }
            else if (otherGo.TryGetComponent<EnemyTemplate>(out EnemyTemplate enemyScript))
            {
                enemyScript.ApplyDamage(intensityProportion * ExplosionDamage);
            }

            // Apply physics force
            if (collider.TryGetComponent<Rigidbody2D>(out Rigidbody2D otherRb))
            {
                float force = intensityProportion * BlastForce;
                Vector2 forceDir = toOther / distance; // Normalized direction (reuse distance)
                otherRb.AddForce(forceDir * force, ForceMode2D.Impulse);
            }
        }
    }
}