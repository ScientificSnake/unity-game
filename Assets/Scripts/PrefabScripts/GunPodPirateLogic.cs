using MathNet.Numerics.Optimization.ObjectiveFunctions;
using Sebastian;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class GunPodPirateLogic : BasicPirateDummyBehaviour
{
    private int IgnoredLayer;

    private string State;
    public GameObject PlayerRef;
    public Rigidbody2D PlayerRb;
    public PolygonCollider2D pcollider;
    public Transform PlayerTransform;

    private float DetectionDistance = 200;
    private bool SeesPlayer;
    private float rotationDegreesPerSeconds = 60;
    private float aimingThreshold = 5f; // Degrees within target to start firing

    [SerializeField] private Sebastian.WeaponryData.Weapon Weapon;
    private WeaponryData.WeaponParameters WeaponParams;
    private WeaponryData.WeaponParameters CurrentWeaponArgs;

    private int ClipAmmo;
    private bool LeftGun;
    private float LastFireTimeStamp;

    private Vector2 RotateVectorByAngle(Vector2 sourceVector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * sourceVector;
    }

    private void RotateTowardsTarget(float zTargetRotation)
    {
        float maxDegreesPerTick = rotationDegreesPerSeconds * Time.fixedDeltaTime;

        float currentAngle = transform.eulerAngles.z;
        float headingMouseAngleDiff = Mathf.DeltaAngle(currentAngle, zTargetRotation);

        float angleTurned;
        if (Mathf.Abs(headingMouseAngleDiff) <= maxDegreesPerTick)
        {
            angleTurned = headingMouseAngleDiff;
        }
        else
        {
            angleTurned = maxDegreesPerTick * Mathf.Sign(headingMouseAngleDiff);
        }

        transform.Rotate(0, 0, angleTurned);
    }

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

    private bool IsAimedAtTarget(float targetAngle)
    {
        float currentAngle = transform.eulerAngles.z;
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        return angleDifference <= aimingThreshold;
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

    protected override void Start()
    {
        base.Start();
        State = "trackPlayer";

        Weapon = Sebastian.WeaponryData.WeaponDict[4];
        rb = gameObject.GetComponent<Rigidbody2D>();
        WeaponParams = Weapon.BaseWeaponParams;
        CurrentWeaponArgs = WeaponParams;

        pcollider = gameObject.GetComponent<PolygonCollider2D>();
        CurrentWeaponArgs.IgnoredColliders = new System.Collections.Generic.List<Collider2D> { pcollider };
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
                    ProjInterceptCalc.InterceptData InterceptInfo = ProjInterceptCalc.TryGetInterceptAngle(
                        transform.position,
                        rb.linearVelocity,
                        PlayerRef.transform.position,
                        PlayerRb.linearVelocity,
                        WeaponParams.MuzzleVelo
                    );

                    if (InterceptInfo.Possible)
                    {
                        float targetAngle = InterceptInfo.AimDeg + 180;
                        RotateTowardsTarget(targetAngle);

                        // Fire when aimed within threshold
                        if (IsAimedAtTarget(targetAngle))
                        {
                            Fire();
                        }
                    }
                }
            }
        }
    }
}