using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;

public class KamikazeEnemyAI : EnemyTemplate
{
    public float PlayerDetectionRadius;
    public string State = "LockedPlayer";
    float SpriteAngleOffset = 0;
    public AudioClip BoomBoomSound;
    public AudioSource Audio;
    protected void RotateTowardsPlayer()
    {
        Vector2 vectorToPlayer = (Vector2)(PlayerRef.transform.position - transform.position);
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg + SpriteAngleOffset;

        // For 2D, rotate around Z-axis
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    protected void UpdateState()
    {
        print($"Dihtance to player is {Vector2.Distance(PlayerRef.transform.position, transform.position)}, Line of sight is {ObjTools.LineOfSight(gameObject, PlayerRef.transform, PlayerDetectionRadius)}");
        if ((Vector2.Distance(transform.position, PlayerRef.transform.position) < PlayerDetectionRadius) && (ObjTools.LineOfSight(gameObject, PlayerRef.transform, PlayerDetectionRadius)))
        {
            State = "LockedPlayer";
        }
        else
        {
            State = "Wandering";
        }
        
    }

    protected void FixedUpdate()
    {
        UpdateState();
        if (State == "LockedPlayer")
        {
            RotateTowardsPlayer();
            Throttle = 50;
        }
        else
        {
            Throttle = 0;
        }
        print("Applying throttle on " + gameObject.name);
        print($" Thrtle : {Throttle}, fuel : {Fuel}");
        ObjTools.ApplyThrottle(Throttle, ref Fuel, MaxAccel, rb, transform, FuelUsage);
    }

    protected void Awake()
    {
        Fuel = 50 / Time.fixedDeltaTime;
        FuelUsage = 1;
        MaxAccel = 200;
        PlayerDetectionRadius = 2000;

        Audio = gameObject.AddComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGo = collision.gameObject;
        if (OtherGo.CompareTag("Player"))
        {
            if (collision.relativeVelocity.magnitude > 20)
            {
                Audio.PlayOneShot(BoomBoomSound, 0.2f);
            }
        }
    }
}