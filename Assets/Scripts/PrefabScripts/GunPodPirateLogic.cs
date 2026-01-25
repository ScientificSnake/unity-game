using MathNet.Numerics;
using Sebastian;
using System.Collections;
using UnityEngine;

public class GunPodPirateLogic : EnemyTemplate
{
    public enum State
    {
        trackPlayer,
        moveToPlayer,
        stabilizing
    }

    public State state;
    public Rigidbody2D PlayerRb;
    public PolygonCollider2D pcollider;
    public Transform PlayerTransform;

    private float DetectionDistance = 1600;
    public float ShootRange;
    public bool SeesPlayer;
    private float aimingThreshold = 5f; // Degrees within target to start firing

    private float stabilizationThreshold = 120;

    protected override void Awake()
    {
        state = State.moveToPlayer;
        thisThrusterSet = Thrusters.GunPodPirateThrusterSet;
        base.Awake();
    }

    [SerializeField] private Sebastian.WeaponryData.Weapon Weapon;
    private WeaponryData.WeaponParameters WeaponParams;
    private WeaponryData.WeaponParameters CurrentWeaponArgs;

    private int ClipAmmo;
    private bool LeftGun;
    private float LastFireTimeStamp;

    private void CheckSeesPlayer()
    {
        if (ObjTools.LineOfSight(gameObject, PlayerTransform, DetectionDistance))
        {
            SeesPlayer = true;
        }
        else
        {
            SeesPlayer = false;
        }
    }

    private void Fire()
    {
        print($"<color=yellow> Firing shotgun boi");
        float RPS = WeaponParams.RPM / 60f;
        float WaitTimeBetweenRounds = 1f / RPS;
        float now = Time.time;
        float timeDiff = now - LastFireTimeStamp;

        if (timeDiff >= WaitTimeBetweenRounds)
        {
            float heading = transform.eulerAngles.z;
            Vector2 offset = Vector2.zero;
            Vector2 offsetVector = RotateVectorByAngle(offset, heading);
            Vector2 pos = new Vector2(transform.position.x, transform.position.y) + offsetVector;

            CurrentWeaponArgs.SpawnPos = pos;
            CurrentWeaponArgs.ParentZRotation = heading;
            CurrentWeaponArgs.ParentVelo = rb.linearVelocity;

            Weapon.SpawnPrefab(CurrentWeaponArgs);
            LastFireTimeStamp = now;
        }
    }

    [SerializeField] private int WeaponIndex;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(PeriodicallyCheckLineOfSightOnPlayer(0.25f));

        Weapon = Sebastian.WeaponryData.WeaponDict[WeaponIndex];
        WeaponParams = Weapon.BaseWeaponParams;
        CurrentWeaponArgs = WeaponParams;

        CurrentWeaponArgs.Spawner = gameObject;

        pcollider = gameObject.GetComponent<PolygonCollider2D>();
        CurrentWeaponArgs.IgnoredColliders = new System.Collections.Generic.List<Collider2D> { pcollider };

        rotationDegreesPerSeconds = 60;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (SeesPlayer && HasPatrol)
        {
            StopPatrol();
        }

        // Cache player references if null
        if (PlayerRef == null || PlayerRb == null)
        {
            PlayerRef = GameObject.FindWithTag("Player");
            if (PlayerRef != null)
            {
                PlayerRb = PlayerRef.GetComponent<Rigidbody2D>();
                PlayerTransform = PlayerRef.GetComponent<Transform>();
            }
            return; // Exit early if player not found
        }
        if (state == State.trackPlayer)
        {
            Throttle = 0;
            float distanceFromPlayer = Vector2.Distance(transform.position, PlayerRef.transform.position);

            if (distanceFromPlayer <= ShootRange)
            {
                if (SeesPlayer)
                {
                    ObjTools.InterceptData InterceptInfo = ObjTools.TryGetInterceptAngle(
                        transform.position,
                        rb.linearVelocity,
                        PlayerRef.transform.position,
                        PlayerRb.linearVelocity,
                        WeaponParams.MuzzleVelo
                    );

                    if (InterceptInfo.Possible)
                    {
                        float targetAngle = InterceptInfo.AimDeg;
                        RotateTowardsTargetAngle(targetAngle);

                        // Fire when aimed within threshold
                        if (IsAimedAtTarget(targetAngle, aimingThreshold))
                        {
                            Fire();
                        }
                    }
                }
            }
            else
            {
                state = State.moveToPlayer;
            }
        }
        else if (state == State.stabilizing)
        {
        }
        else if (state == State.moveToPlayer)
        {
            if (SeesPlayer)
            {
                Vector2 vectorToPlayer = PlayerScriptRef.rb.position - rb.position;
                if (vectorToPlayer.magnitude <= ShootRange)
                {
                    if (rb.linearVelocity.magnitude >= stabilizationThreshold)
                    {
                        state = State.stabilizing;
                        Stabilize();
                    }
                    else
                    {
                        state = State.trackPlayer;
                    }
                }
                else
                {
                    float angleToPlayer = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
                    RotateTowardsTargetAngle(angleToPlayer);
                    Throttle = 100;
                }
            }
        }
    }

    protected IEnumerator PeriodicallyCheckLineOfSightOnPlayer(float period)
    {
        while (true)
        {
            yield return new WaitForSeconds(period);
            CheckSeesPlayer();
        }
    }

    protected IEnumerator Stabilize()
    {
        state = State.stabilizing;
        float stoppingAngleDeg = Mathf.Rad2Deg * Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.y);
        RotateTowardsTargetAngle(stoppingAngleDeg);

        while (!Mathf.Approximately(rb.rotation, stoppingAngleDeg))
        {
            yield return null;
        }

        Throttle = 100;

        while (!Mathf.Approximately(rb.linearVelocity.magnitude, 0))
        {
            yield return null;
        }

        Throttle = 0;

        state = State.moveToPlayer;
    }
}