using Sebastian;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GunPodPirateLogic : BasicPirateDummyBehaviour
{
    private string State;
    public GameObject PlayerRef;
    public Rigidbody2D PlayerRb;
    private float DetectionDistance = 200;

    private Sebastian.WeaponryData.Weapon Weapon;

    private WeaponryData.WeaponParameters WeaponParams;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Vector2 RotateVectorByAngle(Vector2 sourceVector, float angle)
    {
        Vector2 _rotatedVector = Quaternion.AngleAxis(angle, Vector3.forward) * sourceVector;
        return _rotatedVector;
    }

    private int ClipAmmo;
    private bool LeftGun;
    private float LastFireTimeStamp;

    private WeaponryData.WeaponParameters CurrentWeaponArgs;
    private void Fire()
    {
        float RPS = WeaponParams.RPM / 60;
        float WaitTimeBetweenRounds = 1 / RPS;

        float now = Time.time;

        float timeDiff = now - LastFireTimeStamp;

        print($"Wait time befween rounds is {WaitTimeBetweenRounds}");

        if (timeDiff > WaitTimeBetweenRounds)
        {
            float heading = transform.eulerAngles.z + 180;

            Vector2 offset = new(50, 0);

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
    }

    private void FixedUpdate()
    {
        if (PlayerRef == null || PlayerRb == null)
        {
            PlayerRef = GameObject.FindWithTag("Player");
            PlayerRb = PlayerRef.GetComponent<Rigidbody2D>();
        }
        else
        {
            if (State == "trackPlayer")
            {
                float distanceFromPlayer = Vector2.Distance(transform.position, PlayerRef.transform.position);
                if (distanceFromPlayer <= DetectionDistance)
                {
                    //// Debug prints for all TryGetInterceptAngle parameters
                    //Debug.Log($"TryGetInterceptAngle param shooterPos: {transform.position}");
                    //Debug.Log($"TryGetInterceptAngle param shooterVel: {rb.linearVelocity}");
                    //Debug.Log($"TryGetInterceptAngle param targetPos: {PlayerRef.transform.position}");
                    //Debug.Log($"TryGetInterceptAngle param targetVel: {PlayerRb.linearVelocity}");
                    //Debug.Log($"TryGetInterceptAngle param muzzleVelo: {Weapon.BaseWeaponParams.MuzzleVelo}");

                    ProjInterceptCalc.InterceptData InterceptInfo = ProjInterceptCalc.TryGetInterceptAngle(transform.position, rb.linearVelocity, PlayerRef.transform.position, PlayerRb.linearVelocity, WeaponParams.MuzzleVelo);

                    if (InterceptInfo.Possible)
                    {
                        transform.eulerAngles = new Vector3(0, 0, InterceptInfo.AimDeg + 180);
                        Fire();
                    }
                }
            }
        }
    }
}
