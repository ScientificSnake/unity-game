using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    public float Fuel;//time the rocket accelerates for
    public float Acceleration;//rate at which the rocket accelerates
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;

    public Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Fuel > 0)
        {
            Vector2 currentVelo = rb.linearVelocity;
            rb.AddForce(currentVelo.normalized * Acceleration);
            Fuel--;
        }
        if (Fuel == 0)
        {
            spriteRenderer.sprite = newSprite;
        }
    }
}
