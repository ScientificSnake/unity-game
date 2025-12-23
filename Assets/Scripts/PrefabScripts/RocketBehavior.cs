using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    public float Fuel;//time the rocket accelerates for
    public float Acceleration;//rate at which the rocket accelerates
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    public float Damage;
    public PolygonCollider2D tcollider;

    public Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        tcollider = GetComponent<PolygonCollider2D>();
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        tcollider = GetComponent<PolygonCollider2D>();

        print($"Newly inited rocket with velo : {rb.linearVelocity.ToString()}, acceleration {Acceleration}, fuel {Fuel}");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ( tcollider == null )
            tcollider = GetComponent<PolygonCollider2D>();
        if (Fuel > 0)   
        {
            Vector2 Direction = transform.right.normalized;

            rb.AddForce(Direction * Acceleration);
            Fuel--;
        }
        if (Fuel == 0)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("ZE ROCKET HAS HIT ");


        GameObject OtherGo = collision.gameObject;

        if (OtherGo.TryGetComponent<EnemyTemplate>(out EnemyTemplate targetScript))
        {
            targetScript.ApplyDamage(Damage);
        }

        Destroy(gameObject);
    }

    public void ActivateCollider()
    {
        tcollider.enabled = true;
    }
}
