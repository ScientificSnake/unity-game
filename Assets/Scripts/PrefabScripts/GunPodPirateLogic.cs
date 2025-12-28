using MathNet.Numerics.Optimization.ObjectiveFunctions;
using Sebastian;
using UnityEngine;

public class GunPodPirateLogic : EnemyTemplate
{
    private string State;
    public Rigidbody2D PlayerRb;
    public PolygonCollider2D pcollider;
    public Transform PlayerTransform;

    private float DetectionDistance = 420;
    private bool SeesPlayer;
    private float aimingThreshold = 5f; // Degrees within target to start firing

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
        float RPS = WeaponParams.RPM / 60f;
        float WaitTimeBetweenRounds = 1f / RPS;
        float now = Time.time;
        float timeDiff = now - LastFireTimeStamp;

        if (timeDiff >= WaitTimeBetweenRounds)
        {
            float heading = transform.eulerAngles.z + 180;
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

    protected void Start()
    {
        State = "trackPlayer";

        Weapon = Sebastian.WeaponryData.WeaponDict[4];
        rb = gameObject.GetComponent<Rigidbody2D>();
        WeaponParams = Weapon.BaseWeaponParams;
        CurrentWeaponArgs = WeaponParams;

        CurrentWeaponArgs.Spawner = gameObject;

        pcollider = gameObject.GetComponent<PolygonCollider2D>();
        CurrentWeaponArgs.IgnoredColliders = new System.Collections.Generic.List<Collider2D> { pcollider };

        rotationDegreesPerSeconds = 60;
    }

    private void FixedUpdate()
    {
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

        if (State == "trackPlayer")
        {
            float distanceFromPlayer = Vector2.Distance(transform.position, PlayerRef.transform.position);

            if (distanceFromPlayer <= DetectionDistance)
            {
                CheckSeesPlayer();

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
                        float targetAngle = InterceptInfo.AimDeg + 180;
                        RotateTowardsTargetAngle(targetAngle);

                        // Fire when aimed within threshold
                        if (IsAimedAtTarget(targetAngle, aimingThreshold))
                        {
                            Fire();
                        }
                    }
                }
            }
        }
    }
}