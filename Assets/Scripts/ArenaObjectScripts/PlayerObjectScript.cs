using System.Collections;
using System.Net.NetworkInformation;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UIElements;

public class PlayerObjectScript : MonoBehaviour
{
    public float maxAcceleration; // acceleration at 100% throttle
    public float maxReverseAcceleration;  // reverse acceleration at -30% throttle
    public float maxTurnSpeedDPS; // max turn speed in degrees per second

    public float throttle;  // Pre deadzone throttle  Clamped at 100 and -30

    public Vector2 velocity;

    public InArenaControls inputManager;
    private void OnEnable()
    {
        inputManager = new InArenaControls();

        inputManager.Player.LAlt.performed += LAlt_performed;

        inputManager.Enable();
    }

    private void LAlt_performed(InputAction.CallbackContext context)
    {
        print("throttle to 0----------------------------------------------------------------");

        throttle = 0;
    }

    private float RadToDeg(float degree)
    {
        return Mathf.Rad2Deg * degree;
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
            if (throttle < -29 && throttle > -30)
            {
                throttle = -30;
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
        else if (throttle < -30)
        {
            throttle = -30;
        }
    }

    private void ApplyThrottle()
    {
        // dead zone
        if (throttle > 15)
        {
            float trueThrottleProportion = (throttle - 15) / 85;

            //print($"true throttle proportion is {trueThrottleProportion}");

            float instantaneousAcceleration = trueThrottleProportion * maxAcceleration;
            //print($"max acceleration is {maxAcceleration}");
            //print($"instant acceleration is {instantaneousAcceleration}");

            float heading_rad = DegToRadian(transform.eulerAngles.z);

            Vector2 instantaneousAccelerationVector = new Vector2((Mathf.Cos(heading_rad)*instantaneousAcceleration), (Mathf.Sin(heading_rad)*instantaneousAcceleration));

            //print($"Y component of instant acceleration is {Mathf.Asin(heading_rad)} x {instantaneousAcceleration}");
            //print($"X component of instant acceleration is {Mathf.Acos(heading_rad)} x {instantaneousAcceleration}");
            //print($"INSTANT ACCEL vector IS " + instantaneousAccelerationVector.ToString());

            velocity += instantaneousAccelerationVector;
        }
        else if (throttle < -10)
        {
            float heading_rad = DegToRadian(transform.eulerAngles.z);

            float trueReverseThrottleProportion = (Mathf.Abs(throttle) - 10) / 20;
            float instantaneousReverseAcceleration = trueReverseThrottleProportion * maxReverseAcceleration;
            Vector2 instantaneousReverseAccelerationVector = new Vector2(Mathf.Cos(heading_rad) * instantaneousReverseAcceleration, Mathf.Sin(heading_rad) * instantaneousReverseAcceleration);

            velocity += instantaneousReverseAccelerationVector;

        }
    }

    private void ApplyVelocity()
    {
        Vector3 Velocity3d = new(velocity.x, velocity.y, 0);

        transform.position += Velocity3d;
    }

    private void HeadingFollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;

        // Calculate mouse angle from center
        Vector2 mouseOffset = new(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2);
        float targetAngle = Mathf.Atan2(mouseOffset.y, mouseOffset.x) * Mathf.Rad2Deg;

        // Calculate the shortest angular difference
        float currentAngle = transform.eulerAngles.z;
        float headingMouseAngleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

        //print($"Target angle: {targetAngle}, Current: {currentAngle}, Diff: {headingMouseAngleDiff}");

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
        velocity.x = 0;
        velocity.y = 0;
        maxAcceleration = 0.05f;
        maxReverseAcceleration = -0.01f;
        maxTurnSpeedDPS = 180;
    }

    private void FixedUpdate()
    {
        ThrottleControl();
        ApplyThrottle();
        ApplyVelocity();
        HeadingFollowMouse();
    }

    // Update is called once per frame
    void Update()
    {
    }
};
