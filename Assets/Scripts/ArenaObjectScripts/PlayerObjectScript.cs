using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using Sebastian;
using NUnit.Framework.Constraints;

public class PlayerObjectScript : MonoBehaviour
{
    public Rigidbody2D rb;
    private PolygonCollider2D PolygonCollider;
    private SpriteRenderer spriteRenderer;

    public AudioSource audios;
    public AudioClip collisionSound;
    public AudioClip ShotSound;

    public float FuelUsage = 1;

    public float CollsionDamageMultiplier = 0.02f;

    public LevelDataStorage.LevelManager.BaseStats BaseRoundStats;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.angularDamping = 0;
        rb.linearDamping = 0;
        rb.freezeRotation = true;
        audios = GetComponent<AudioSource>();
    }

    #region public vars

    public List<Vector2> ThrusterBaseScales;
    public List<GameObject> ThrusterRefs;

    public float Fuel;
    public float maxAcceleration; // acceleration at 100% throttle
    public float maxTurnSpeedDPS; // max turn speed in degrees per second

    public Vector2 Offset;

    public Action<Sebastian.WeaponryData.WeaponParameters> SpawnMainWeaponPrefabAction;
    public WeaponryData.WeaponParameters CurrentMainWeaponParams;

    private float LastFireTimeStamp;

    public float throttle;  // Pre deadzone throttle
    public float Health;

    public Camera mainCamera;

    public InArenaControls inputManager;
    #endregion
    private void OnEnable()
    {
        inputManager = new InArenaControls();

        inputManager.Player.LAlt.performed += LAlt_performed;

        inputManager.Enable();
    }

    private void LAlt_performed(InputAction.CallbackContext context)
    {
        throttle = 0;
    }

    private Vector2 RotateVectorByAngle(Vector2 sourceVector, float angle)
    {
        Vector2 _rotatedVector = Quaternion.AngleAxis(angle, Vector3.forward) * sourceVector;
        return _rotatedVector;
    }

    private void PollMainWeapon()
    {
        if (inputManager.Player.LClick.IsPressed())
        {

            // calculate wait time

            float RPS = CurrentMainWeaponParams.RPM / 60;
            float WaitTimeBetweenRounds = 1 / RPS;

            // Calculate difference between now and last firing

            float now = Time.time;

            float timeDiff = now - LastFireTimeStamp;

            if (timeDiff >= WaitTimeBetweenRounds)
            {
                // arguments for calling the function

                float heading = transform.eulerAngles.z;

                Vector2 rotatedOffset = RotateVectorByAngle(Offset, transform.eulerAngles.z);

                Vector2 pos = new Vector2(transform.position.x, transform.position.y) + rotatedOffset;

                CurrentMainWeaponParams.SpawnPos = pos;
                CurrentMainWeaponParams.ParentZRotation = heading;
                CurrentMainWeaponParams.ParentVelo = rb.linearVelocity;

                SpawnMainWeaponPrefabAction(CurrentMainWeaponParams);

                LastFireTimeStamp = now;

                float heading_rad = Mathf.Deg2Rad * transform.eulerAngles.z;

                Vector2 instantaneousAccelerationVector = new Vector2((Mathf.Cos(heading_rad) * CurrentMainWeaponParams.RecoilForce), (Mathf.Sin(heading_rad) * CurrentMainWeaponParams.RecoilForce));

                rb.AddForce(instantaneousAccelerationVector);

                audios.PlayOneShot(ShotSound);
            }
        }
    }

    private void OnDisable()
    {
        inputManager.Disable();
    }

    private void ThrottleControl()
    {
        if (inputManager.Player.LShift.IsPressed())
        {
            if (throttle > 99 && throttle < 100)
            {
                throttle = 100;
            }
            else
            {
                throttle += 5;
            }
        }

        if (inputManager.Player.LControl.IsPressed())
        {
            if (throttle < 1 && throttle > 0)
            {
                throttle = 0;
            }
            else
            {
                throttle -= 5;
            }
        }


        if (throttle > 100)
        {
            throttle = 100;
        }
        else if (throttle < 0)
        {
            throttle = 0;
        }
    }
    
    //private void ScaleThrusters()
    //{
    //    for (int i = 0; i < ThrusterRefs.Count; i++)
    //    {
    //        Vector2 targetScale = ThrusterBaseScales[i] * (throttle/100);
    //        ThrusterRefs[i].gameObject.transform.localScale = targetScale;
    //    }
    //}
    private void HeadingFollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;

        Vector2 screenPosition = mainCamera.WorldToScreenPoint(transform.position);

        // Calculate mouse angle from center
        Vector2 mouseOffset = new(mousePos.x - screenPosition.x, mousePos.y - screenPosition.y);
        float targetAngle = Mathf.Atan2(mouseOffset.y, mouseOffset.x) * Mathf.Rad2Deg;

        // Calculate the shortest angular difference
        float currentAngle = transform.eulerAngles.z;
        float headingMouseAngleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Calculate max rotation
        float maxDegreesPerTick = maxTurnSpeedDPS * Time.fixedDeltaTime;

        // Determine how much to actually rotate
        float angleTurned;
        if (Mathf.Abs(headingMouseAngleDiff) <= maxDegreesPerTick)
        {
            // We can reach the target this tick
            angleTurned = headingMouseAngleDiff;
        }
        else
        {
            // Rotate max amount in the correct direction
            angleTurned = maxDegreesPerTick * Mathf.Sign(headingMouseAngleDiff);
        }

        transform.Rotate(0, 0, angleTurned);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        #region Collider setup

        spriteRenderer = GetComponent<SpriteRenderer>();

        PolygonCollider = GetComponent<PolygonCollider2D>();
        Destroy(PolygonCollider);

        PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();

        // Unity 6 method for creating collider from sprite
        List<Vector2> path = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, path);
        collider.pathCount = 1;
        collider.SetPath(0, path);
        #endregion

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        #region initialize stats based on what hull is chosen

        // BaseRoundStats = ManagerScript.CurrentLevelManagerInstance.Stats;
        // string hullSysName = ManagerScript.CurrentLevelManagerInstance.selectedHull;

        ApplyRoundStats();

        #endregion

        #region Weapon intialization

        int WeaponIndex = (int)BaseRoundStats.WeaponSelection;

        Sebastian.WeaponryData.Weapon TargetWeapon = Sebastian.WeaponryData.WeaponDict[WeaponIndex];

        SpawnMainWeaponPrefabAction = TargetWeapon.SpawnPrefab;

        CurrentMainWeaponParams = TargetWeapon.BaseWeaponParams;
        CurrentMainWeaponParams.IgnoredColliders = new List<Collider2D> { collider };
        #endregion

        #region initialize thrusters
        ThrusterRefs = Thrusters.ApplyThrusterSet(gameObject, BaseRoundStats.thrusterLayout);
        
        foreach (Thrusters.Thruster thruster in BaseRoundStats.thrusterLayout.thrusters)
        {
            ThrusterBaseScales.Add(thruster.baseScale);
        }
        #endregion
    }

    public void ApplyRoundStats()
    {
        maxTurnSpeedDPS = BaseRoundStats.MaxTurnRate;
        maxAcceleration = BaseRoundStats.Acceleration / 40;  // Dividing by 40 because Per second -> Per tick 40 tps
        rb.mass = BaseRoundStats.Mass;
        transform.localScale = new Vector3(BaseRoundStats.ScaleFactor, BaseRoundStats.ScaleFactor);
        Fuel = BaseRoundStats.BaseFuel * 40; // Same reasoning as above ^^^^^ but this time now it is fuel usage for each tick
        Offset = BaseRoundStats.GunOffset;
        Health = BaseRoundStats.Health;
    }

    public void ResetRoundStats()
    {

        print("reseting stats to round start");
        ApplyRoundStats();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        throttle = 0;
        Fuel = BaseRoundStats.BaseFuel / Time.fixedDeltaTime;
        transform.position = Vector3.zero;
    }

    private void FixedUpdate()
    {
        ThrottleControl();
        ObjTools.ApplyThrottle(throttle, ref Fuel, maxAcceleration, rb, transform, FuelUsage);
        HeadingFollowMouse();
        PollMainWeapon();
        ObjTools.ScaleThrusterRefs(ThrusterRefs, ThrusterBaseScales, throttle);
    }

    #region Collision damage handling
        
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float relativeVelocityMagnitude = collision.relativeVelocity.magnitude;

        //print($"Collided with {collision.gameObject.name}, at relative velo of {collision.relativeVelocity.magnitude}");

        if (collision.relativeVelocity.magnitude > 50)
        {
            audios.PlayOneShot(collisionSound);
            print("<color=yellow> Player was hit with rel valo of " + relativeVelocityMagnitude);
            ApplyDamage(relativeVelocityMagnitude * CollsionDamageMultiplier);
            HealthCheck();

        }
    }
    #endregion

    #region Health management and Damage application
    public void ApplyDamage(float damageAmount)
    {
        Health -= damageAmount;
        HealthCheck();
        print($"<color=orange> Player now has {Health}, took {damageAmount} damage");
    }

    private void HealthCheck()
    {
        if (Health <= 0)
        {
            ManagerScript.CurrentLevelManagerInstance.GameOver();
            print($"Player has been slimed");
            // Handle player destruction (e.g., trigger game over, respawn, etc.)
        }
    }

    #endregion
}