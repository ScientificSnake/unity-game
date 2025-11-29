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

public class PlayerObjectScript : MonoBehaviour
{
    public float maxAcceleration; // acceleration at 100% throttle
    public float maxReverseAcceleration;  // reverse acceleration at -30% throttle
    public float maxTurnSpeedDPS; // max turn speed in degrees per second

    public Action<Vector2, Vector2, float, float, float> SpawnMainWeaponPrefabAction;
    public float MainWeaponRPM;
    public float MuzzleVelo;

    private float LastFireTimeStamp;

    public float throttle;  // Pre deadzone throttle

    public Vector2 Velocity;
    public float Health;

    public Camera mainCamera;

    // Interpolation variables to fix stuttering
    private Vector3 previousPosition;
    private Vector3 currentPosition;

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

            float RPS = MainWeaponRPM / 60;
            float WaitTimeBetweenRounds = 1 / RPS;

            // Calculate difference between now and last firing

            float now = Time.time;

            float timeDiff = now - LastFireTimeStamp;

            if (timeDiff >= WaitTimeBetweenRounds)
            {
                // arguments for calling the function

                float heading = transform.eulerAngles.z;
                Vector2 pos = currentPosition; // Use currentPosition instead of transform.position
                float Accuracy = 1.0f;

                SpawnMainWeaponPrefabAction(pos, Velocity, heading, MuzzleVelo, Accuracy);

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
                throttle++;
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
                throttle--;
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
        if (throttle > 15)
        {
            float trueThrottleProportion = (throttle - 15) / 85;

            float instantaneousAcceleration = trueThrottleProportion * maxAcceleration;

            float heading_rad = DegToRadian(transform.eulerAngles.z);

            Vector2 instantaneousAccelerationVector = new Vector2((Mathf.Cos(heading_rad)*instantaneousAcceleration), (Mathf.Sin(heading_rad)*instantaneousAcceleration));

            Velocity += instantaneousAccelerationVector;
        }
    }

    private void ApplyVelocity()
    {
        Vector3 Velocity3d = new(Velocity.x, Velocity.y, 0);

        // Store previous position before updating
        previousPosition = currentPosition;
        currentPosition += Velocity3d;
    }

    private void HeadingFollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;
        
        Vector2 screenPosition = mainCamera.WorldToScreenPoint(currentPosition);

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
        Velocity.x = 0;
        Velocity.y = 0;

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        // Initialize positions
        currentPosition = transform.position;
        previousPosition = currentPosition;

        // initialize stats based on what hull is chosen

        Dictionary<string, float> baseStats = ManagerScript.CurrentLevelManagerInstance.BaseStats;
        string hullSysName = ManagerScript.CurrentLevelManagerInstance.selectedHull;

        maxTurnSpeedDPS = baseStats["MaxTurnRate"];
        maxAcceleration = baseStats["Acceleration"] / 40;  // Dividing by 40 because Per second -> Per tick 40 tps

        #region Weapon intialization

        int WeaponIndex = (int) baseStats["WeaponSelection"];

        Sebastian.WeaponryData.Weapon TargetWeapon = Sebastian.WeaponryData.WeaponDict[WeaponIndex];

        SpawnMainWeaponPrefabAction = TargetWeapon.SpawnPrefab;
        MuzzleVelo = TargetWeapon.BaseMuzzleVelocity;
        MainWeaponRPM = TargetWeapon.fireRate;
        LastFireTimeStamp = Time.time;
        #endregion
    }

    private void FixedUpdate()
    {
        ThrottleControl();
        ApplyThrottle();
        ApplyVelocity();
        HeadingFollowMouse();
        PollMainWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        // Interpolate visual position between physics updates
        float t = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        transform.position = Vector3.Lerp(previousPosition, currentPosition, t);
    }
}