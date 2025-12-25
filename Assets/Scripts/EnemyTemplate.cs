using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class EnemyTemplate : MonoBehaviour, IBoundsCheckable
{
    public float Health;
    public Rigidbody2D rb;
    public Rigidbody2D Rigidbody2 => rb;
    public float rotationDegreesPerSeconds;

    public float Fuel;
    public float MaxAccel;
    public float Throttle;
    public float FuelUsage;

    public SpriteRenderer spriteRenderer;
    public GameObject PlayerRef;
    public PlayerObjectScript PlayerScriptRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Health = 1;
        PlayerRef = GameObject.FindWithTag("Player");
        PlayerScriptRef = PlayerRef.GetComponent<PlayerObjectScript>();
    }

    public void ApplyDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            DestructSelf();
        }
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
