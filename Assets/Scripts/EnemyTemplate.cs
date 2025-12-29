using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTemplate : MonoBehaviour, IBoundsCheckable, IMiniMapTrackable
{
    public float Health = 1;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
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

        _InitializeThrusters();
    }

    protected virtual void FixedUpdate()
    {
        if (thisThrusterSet != null)
        {
            ObjTools.ScaleThrusterRefs(ThrusterRefs, ThrusterBaseScales, Throttle);
        }
    }

    private void _InitializeThrusters()
    {
        if (thisThrusterSet != null)
        {
            ThrusterRefs = Thrusters.ApplyThrusterSet(gameObject, thisThrusterSet);
            ThrusterBaseScales = thisThrusterSet.BaseScales;
        }
    }

    public void ApplyDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            DestructSelf();
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
        rb.angularVelocity = 3000;
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
