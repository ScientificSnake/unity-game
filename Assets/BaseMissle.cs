using MathNet.Numerics.Financial;
using UnityEngine;

public class BaseMissle : ProjectileTemplate
{
    [SerializeField] private Sprite spCold;
    [SerializeField] private Sprite spTurnRight;
    [SerializeField] private Sprite spTurnLeft;
    [SerializeField] private Sprite spStraight;
    private SpriteRenderer spRenderer;
    private float turningTorque = 0.01f;
    private float acceleration = 100f;

    protected override void Awake()
    {
        base.Awake();

        angularAcceleration = turningTorque / rb.inertia;
        spRenderer = GetComponent<SpriteRenderer>();
        rb.angularDamping = 0.25f;
        Fuel = 500;
        spRenderer.sprite = spCold;
    }

    protected enum ThrustState
    {
        NoLock,
        TurnRight,
        TurnLeft,
        Straight,
        Cold
    }

    protected ThrustState thisThrustState;

    #region Sprite changes

    public enum MissleSpriteEnum
    {
        Cold,
        TurnLeft,
        TurnRight,
        Straight
    }
    public void ChangeSprite(MissleSpriteEnum targetSprite, Sprite newsprite)
    {
        switch (targetSprite)
        {
            case MissleSpriteEnum.Cold:
                spCold = newsprite;
                break;
            case MissleSpriteEnum.TurnLeft:
                spTurnLeft = newsprite;
                break;
            case MissleSpriteEnum.TurnRight:
                spTurnRight = newsprite;
                break;
            case MissleSpriteEnum.Straight:
                spStraight = newsprite;
                break;
        }
    }

    #endregion

    private float Fuel;

    public Rigidbody2D TargetLock;

    protected void FixedUpdate()
    {
        Vector2 directionToPlayer = TargetLock.position - (Vector2) transform.position;

        GetThrustState(directionToPlayer.DirectionAngle());
        ApplyThrustStateSprite();
        ApplyThrustStatePhysics();
    }

    [SerializeField] private float OnTargetThreshold = 5f;
    [SerializeField] private float HoldVeloTreshold = 1f;

    private void GetThrustState(float targetAngle)
    {
        float deltaAngle = Mathf.DeltaAngle(targetAngle, rb.transform.eulerAngles.z);
        float brakeNowArcMeasure = GetBrakeingArcMeasure();

        if ((Mathf.Abs(deltaAngle) < OnTargetThreshold) && Mathf.Abs(rb.angularVelocity) < HoldVeloTreshold)
        {
            thisThrustState = ThrustState.Straight;
            return;
        }
        
        float diff = deltaAngle - brakeNowArcMeasure;

        print($"<color=green> diff is {diff}");
        print($"<color=orange> angular velo is {rb.angularVelocity}");

        if (diff > OnTargetThreshold)
        {
            thisThrustState = ThrustState.TurnRight;
            print($"<color=orange> Turning right");
        }
        else if (diff < -OnTargetThreshold)
        {
            print($"<color=orange> Turning left");
            thisThrustState = ThrustState.TurnLeft;
        } else
        {
            print($"le diff in ze dead zoney");

            if (rb.angularVelocity < 0)
            {
                thisThrustState = ThrustState.TurnRight;
            } 
            else
            {
                thisThrustState = ThrustState.TurnLeft;
            }

        }
    }
    private void Start()
    {
        TargetLock = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
    }
    private void ApplyThrustStateSprite()
    {
        switch (thisThrustState)
        {
            case ThrustState.Straight:
                spRenderer.sprite = spStraight;
                break;
            case ThrustState.TurnRight:
                spRenderer.sprite = spTurnRight;
                break;
            case ThrustState.TurnLeft:
                spRenderer.sprite = spTurnLeft;
                break;
            case ThrustState.Cold:
                spRenderer.sprite = spCold;
                break;
        }
    }

    private void ApplyThrustStatePhysics()
    {
        switch (thisThrustState)
        {
            case ThrustState.Straight:
                rb.AddForce(transform.right * acceleration);
                Fuel -= 2;
                break;
            case ThrustState.TurnRight:
                rb.AddTorque(turningTorque);
                Fuel -= 0.5f;
                break;
            case ThrustState.TurnLeft:
                rb.AddTorque(-turningTorque);
                Fuel -= 0.5f;
                break;
        }
    }

    public float angularAcceleration;

    private float GetBrakeingArcMeasure()
    {
        float v0 = rb.angularVelocity;
        float d = rb.angularDamping;
        float ba = angularAcceleration;

        float bT = v0 / (ba + (d * v0));
        float breakingArcMeasure = (v0 * bT) + (v0 * d * bT * bT)/2 - (ba * bT * bT)/2;
        return breakingArcMeasure;
    }
}
