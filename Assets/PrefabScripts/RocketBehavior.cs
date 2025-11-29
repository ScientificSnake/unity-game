using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    public float Fuel;//time the rocket accelerates for
    public float Acceleration;//rate at which the rocket accelerates
    public Vector2 velocity;
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 wierdDidle = new(velocity.x, velocity.y, 0);
        accelerates();
        transform.position += wierdDidle;
        if (Fuel == 0)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    private void accelerates()
    {
        
        if(Fuel>0)
        {
            velocity = velocity.normalized * (velocity.magnitude + Acceleration);
            Fuel --;
        }
    }
}
