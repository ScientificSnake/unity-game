using System.Collections;
using System.Net.NetworkInformation;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using Sebastian;

public class PlayerObjectScript : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.angularDamping = 0;
        rb.linearDamping = 0;
    }


    public float Fuel;
    public float maxAcceleration; // acceleration at 100% throttle
    public float maxTurnSpeedDPS; // max turn speed in degrees per second

    public Action<Sebastian.WeaponryData.WeaponParameters> SpawnMainWeaponPrefabAction;
    public WeaponryData.WeaponParameters CurrentMainWeaponParams;

    private float LastFireTimeStamp;

    public float throttle;  // Pre deadzone throttle
    public float Health;

    public Camera mainCamera;

    public InArenaControls inputManager;
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
                Vector2 pos = transform.position;

                CurrentMainWeaponParams.SpawnPos = pos;
                CurrentMainWeaponParams.ParentZRotation = heading;
                CurrentMainWeaponParams.ParentVelo = rb.linearVelocity;

                SpawnMainWeaponPrefabAction(CurrentMainWeaponParams);
                print(SpawnMainWeaponPrefabAction.ToString());

                LastFireTimeStamp = now;
            }
        }
    }

    private float DegToRadian(float rad)
    {
        return Mathf.Deg2Rad * rad;
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

    private void ApplyThrottle()
    {
        // dead zone
        if (throttle > 15 && Fuel > 0)
        {
            float trueThrottleProportion = (throttle - 15) / 85;

            float instantaneousAcceleration = trueThrottleProportion * maxAcceleration;

            float heading_rad = DegToRadian(transform.eulerAngles.z);

            Vector2 instantaneousAccelerationVector = new Vector2((Mathf.Cos(heading_rad)*instantaneousAcceleration), (Mathf.Sin(heading_rad)*instantaneousAcceleration));

            rb.AddForce(instantaneousAccelerationVector);

            float fuelUsage = trueThrottleProportion;
            Fuel -= fuelUsage;
        }
    }

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
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        // initialize stats based on what hull is chosen

        Dictionary<string, float> baseStats = ManagerScript.CurrentLevelManagerInstance.BaseStats;
        string hullSysName = ManagerScript.CurrentLevelManagerInstance.selectedHull;

        print($"{hullSysName}, is trying to initialize;");

        maxTurnSpeedDPS = baseStats["MaxTurnRate"];
        maxAcceleration = baseStats["Acceleration"] / 40;  // Dividing by 40 because Per second -> Per tick 40 tps

        //rb.mass = baseStats["Mass"];
        transform.localScale = new Vector3(baseStats["ScaleFactor"], baseStats["ScaleFactor"], baseStats["ScaleFactor"]);

        Fuel = baseStats["BaseFuel"] * 40; // Same reasoning as above ^^^^^ but this time now it is fuel usage for each tick

        #region Weapon intialization

        int WeaponIndex = (int) baseStats["WeaponSelection"];

        Sebastian.WeaponryData.Weapon TargetWeapon = Sebastian.WeaponryData.WeaponDict[WeaponIndex];
        print($"Target weapon with index {WeaponIndex}, {TargetWeapon}");

        SpawnMainWeaponPrefabAction = TargetWeapon.SpawnPrefab;

        CurrentMainWeaponParams = TargetWeapon.BaseWeaponParams;
        #endregion
    }

    private void FixedUpdate()
    {
        ThrottleControl();
        ApplyThrottle();
        HeadingFollowMouse();
        PollMainWeapon();
    }
}