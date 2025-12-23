using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;

public class EnemyTemplate : MonoBehaviour
{
    public float Health;
    public Rigidbody2D rb;
    public float rotationDegreesPerSeconds;

    public float Fuel;
    public float MaxAccel;
    public float Throttle;

    public GameObject PlayerRef;
    public PlayerObjectScript PlayerScriptRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
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

    public void RotateTowardsTarget(float zTargetRotation)
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
}
