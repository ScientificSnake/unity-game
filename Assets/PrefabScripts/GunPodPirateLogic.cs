using Sebastian;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GunPodPirateLogic : BasicPirateDummyBehaviour
{
    private string State;
    public GameObject PlayerRef;
    public Rigidbody2D PlayerRb;
    public Transform PlayerTransform;
    private float DetectionDistance = 200;
    private bool SeesPlayer;

    [SerializeField] 

    private Sebastian.WeaponryData.Weapon Weapon;

    private WeaponryData.WeaponParameters WeaponParams;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Vector2 RotateVectorByAngle(Vector2 sourceVector, float angle)
    {
        Vector2 _rotatedVector = Quaternion.AngleAxis(angle, Vector3.forward) * sourceVector;
        return _rotatedVector;
    }

    private void CheckSeesPlayer()
    {
        Vector2 directionToPlayer = PlayerTransform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, DetectionDistance);

        print($"Hit {hit.collider.name}");

        if (hit.transform == PlayerTransform.transform)
        {
            SeesPlayer = true;
        }
        else
        {
            SeesPlayer = false;
        }
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
            PlayerTransform = PlayerRef.GetComponent<Transform>();
        }
        else
        {
            if (State == "trackPlayer")
            {
                float distanceFromPlayer = Vector2.Distance(transform.position, PlayerRef.transform.position);
                if (distanceFromPlayer <= DetectionDistance)
                {
                    CheckSeesPlayer();
                    print($"Sees player is {SeesPlayer}");
                    if (SeesPlayer)
                    {
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
}
