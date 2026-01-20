using UnityEngine;

public class BaseMissle : ProjectileTemplate
{
    private Sprite spCold;
    private Sprite spTurnRight;
    private Sprite spTurnLeft;
    private Sprite spStraight;

    private SpriteRenderer spRenderer;

    protected override void Awake()
    {
        base.Awake();
        SpriteRenderer spRenderer = GetComponent<SpriteRenderer>();
        rb.angularDamping = 0.25f;
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
            case MissleSpriteEnum:
                spStraight = newsprite;
                break;
        }
    }

    #endregion

    private float Fuel;

    private Rigidbody2D PlayerLock;

    protected void FixedUpdate()
    {
        Vector2 directionToPlayer = PlayerLock.position - (Vector2) transform.position;

        GetThrustState(directionToPlayer.DirectionAngle());
        UseThrustState();
    }

    private void GetThrustState(float targetAngle)
    {
        float deltaAngle = Mathf.DeltaAngle(targetAngle, rb.transform.eulerAngles.z);

        if (deltaAngle == 0)
        {
            thisThrustState = ThrustState.Straight;
        }
        else if (deltaAngle > 0)
        {
            thisThrustState = ThrustState.TurnRight;
        }
        else if (deltaAngle < 0)
        {
            thisThrustState = ThrustState.TurnLeft;
        }
    }

    private void UseThrustState()
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
}
