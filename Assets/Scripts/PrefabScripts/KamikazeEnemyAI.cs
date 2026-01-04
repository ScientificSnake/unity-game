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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateState();
        if (State == "LockedPlayer")
        {
            ObjTools.InterceptData interceptData = ObjTools.TryGetInterceptAngle(
                transform.position, rb.linearVelocity, PlayerRef.transform.position,
                PlayerScriptRef.rb.linearVelocity, rb.linearVelocity.magnitude);

            float targetAngle;

            if (interceptData.Possible && (PlayerScriptRef.rb.linearVelocity.magnitude > ManualTargetingThreshold))
            {
                targetAngle = interceptData.AimDeg;
            }
            else
            {
                Vector2 toPlayerDir = PlayerRef.transform.position - transform.position;
                targetAngle = Mathf.Atan2(toPlayerDir.y, toPlayerDir.x) * Mathf.Rad2Deg;
            }

            RotateTowardsTargetAngle(targetAngle);
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
    }

    protected override void Awake()
    {
        thisThrusterSet = Thrusters.KamikazeThrusterSet;
        Health = 45;

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

        ObjTools.ApplyExplosionDamage(colliderList, gameObject, DamageRadius, contactFilter, ExplosionDamage, BlastForce);
    }

    
}