using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public Vector2 velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 didle = new(velocity.x, velocity.y, 0);
        transform.position += didle;
    }
}
//haisdfgihhsbdfoyig