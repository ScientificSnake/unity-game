using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class EnemyTemplate : MonoBehaviour, IBoundsCheckable, IMiniMapTrackable
{
    protected AudioSource thisAudio;

    public HealthScript thisHealth;

    public float Health;
    public Rigidbody2D rb;
    public Rigidbody2D Rigidbody2 => rb;

    [Header("Mini map tracking")]
    public Sprite localMiniMapIcon;
    public Sprite MiniMapIcon => localMiniMapIcon;
    public bool SeenByPlayer => true;

    [Header("Movement")]
    public float Fuel;
    public float MaxAccel;
    public float Throttle;
    public float FuelUsage;
    public float rotationDegreesPerSeconds;

    public SpriteRenderer spriteRenderer;

    [Header("References")]
    public static GameObject PlayerRef;
    public static PlayerObjectScript PlayerScriptRef;

    protected Thrusters.ThrusterSet thisThrusterSet;
    //private List<Vector2> ThrusterBaseScale;
    private List<GameObject> ThrusterRefs;
    private List<Vector2> ThrusterBaseScales;

    private GameObject SparksPrefab;

    private List<Vector2> InitialWayPoints;
    [SerializeField] protected List<GameObject> WayPoints;
    private Coroutine PatrolCoRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        SparksPrefab = ManagerScript.Instance.BasicSparksPrefab;

        InitialWayPoints = new List<Vector2>();

        thisAudio = gameObject.AddComponent<AudioSource>();

        thisHealth = gameObject.GetOrAddComponent<HealthScript>();
        thisHealth.OnDamage = SpawnSparks;
        thisHealth.OnDeath = DestructSelf;

        thisHealth.Health = Health;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (PlayerRef == null)
        {
            PlayerRef = GameObject.FindWithTag("Player");
            PlayerScriptRef = PlayerRef.GetComponent<PlayerObjectScript>();
        }
        BoundsEnforcer.Register(this);
        MiniMapRegister.Register(this);
        rb.linearDamping = 0.25f;

        InitializeThrusters();
    }

    protected virtual void Start()
    {
        if (WayPoints.Count > 1)
        {
            foreach (GameObject waypoint in WayPoints)
            {
                if (waypoint != null)
                    InitialWayPoints.Add(waypoint.transform.position);
            }

            PatrolCoRoutine = StartCoroutine(PatrolPoint());
            HasPatrol = true;
        }
        else if (WayPoints.Count == 1)
        {
            StartCoroutine(MoveTo(WayPoints[0].transform.position));
            HasPatrol = false;
        }
        else
        {
            HasPatrol = false;
        }
    }

    private float stoppedThreshold = 3;
    private float closeEnough = 2;

    protected List<Coroutine> PatrolCoRoutines = new();

    protected bool HasPatrol;

    protected void StopPatrol()
    {
        foreach(Coroutine e in PatrolCoRoutines)
        {
            StopCoroutine(e);
        }
        StopCoroutine(PatrolCoRoutine);
    }

    protected IEnumerator PatrolPoint()
    {
        print($"<color=yellow>started patrol");
        while (true)
        {
            for (int i = 0; i < InitialWayPoints.Count; i++)
            {
                Coroutine moveCoRoutine = StartCoroutine(MoveTo(InitialWayPoints[i]));
                PatrolCoRoutines.Add(moveCoRoutine);

                yield return moveCoRoutine;

                PatrolCoRoutines.Remove(moveCoRoutine);

                print($"<color=green> finished waypoint {i}");
            }
        }
    }

    protected IEnumerator MoveTo(Vector2 target)
    {
        //first stabilize
        float newHeading;
        if (rb.linearVelocity.magnitude > 3)
        {
            newHeading = rb.linearVelocity.DirectionAngle() + 180;
            while (true)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, newHeading)) < closeEnough)
                {
                    break;
                }
                RotateTowardsTargetAngle(newHeading);
                yield return null;
            }

            Throttle = 100;

            while (true)
            {
                if (Mathf.Abs(rb.linearVelocity.magnitude) < stoppedThreshold)
                {
                    break;
                } 
                yield return null;
            }
            Throttle = 0;
        }

        Vector2 VectorToTarget = target - (Vector2) transform.position;

        newHeading = VectorToTarget.DirectionAngle();

        while (true)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, newHeading)) < closeEnough)
            {
                break;
            }
            yield return null;
            RotateTowardsTargetAngle(newHeading);
        }

        Throttle = 100;

        float targetDistance;
        float stopNowGlideDist;
        float stopNowGlideTime;

        while (true)
        {
            targetDistance = Vector2.Distance(target, transform.position);
            stopNowGlideTime = 1 / rb.linearDamping;
            stopNowGlideDist = (stopNowGlideTime * rb.linearVelocity.magnitude) - (rb.linearDamping * (Mathf.Pow(stopNowGlideTime, 2) / 2));

            if (Mathf.Abs(stopNowGlideDist - targetDistance) < closeEnough)
            {
                Throttle = 0;
                break;
            }
            else if (stopNowGlideDist >= targetDistance)
            {
                Throttle = 0;
                break;
            }

            yield return null;
        }

        while (true)
        {
            targetDistance = Vector2.Distance(target, transform.position);
            if (targetDistance < closeEnough * 2)
            {
                break;
            }
            yield return null;
        }
    }

    public void SpawnSparks()
    {
        GameObject sparks = Instantiate(SparksPrefab);
        sparks.transform.position = transform.position;
        sparks.GetComponent<Rigidbody2D>().linearVelocity = rb.linearVelocity;
    }

    protected virtual void FixedUpdate()
    {
        if (thisThrusterSet != null)
        {
            ObjTools.ApplyThrottle(Throttle, ref Fuel, MaxAccel, rb, transform, FuelUsage);
            ObjTools.ScaleThrusterRefs(ThrusterRefs, ThrusterBaseScales, Throttle);
        }
    }

    private void InitializeThrusters()
    {
        if (thisThrusterSet != null)
        {
            ThrusterRefs = Thrusters.ApplyThrusterSet(gameObject, thisThrusterSet);
            ThrusterBaseScales = thisThrusterSet.BaseScales;
        }
    }

    protected void OnDestroy()
    {
        MiniMapRegister.DeRegister(this);
        BoundsEnforcer.DeRegister(this);
    }

    public Vector2 RotateVectorByAngle(Vector2 sourceVector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * sourceVector;
    }
    protected bool IsAimedAtTarget(float targetAngle, float aimingThreshold)
    {
        float currentAngle = transform.eulerAngles.z;
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        return angleDifference <= aimingThreshold;
    }

    public void RotateTowardsTargetAngle(float zTargetRotation)
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

    protected void DestructSelf()
    {
        if (!(TryGetComponent<AudioSource>(out AudioSource audio)))
        {
            gameObject.AddComponent<AudioSource>();
        }
        audio = gameObject.GetComponent<AudioSource>();

        //audio.PlayOneShot()

        MiniMapRegister.DeRegister(this);
        Color color = spriteRenderer.color;
        color.r = 1;
        color.g = 1;
        color.b = 1;

        spriteRenderer.color = color;

        StartCoroutine(DestroyInSeconds(0.5f));
    }

    private IEnumerator DestroyInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    protected IEnumerator RunOnDelayCR(Action code, float delay)
    {
        yield return new WaitForSeconds(delay);
        code();
    }

    public void OnOutOfBounds()
    {
        BoundsEnforcer.DeRegister(this);
        Destroy(gameObject);
    }
}
