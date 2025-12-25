using UnityEngine;

public class ProjectileTemplate : MonoBehaviour, IBoundsCheckable
{
    public Rigidbody2D rb;
    public Rigidbody2D Rigidbody2 => rb;
    public Collider2D tcollider;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tcollider = GetComponent<Collider2D>();
        BoundsEnforcer.Register(this);
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.gravityScale = 0f;
    }

    public void OnOutOfBounds()
    {
        BoundsEnforcer.DeRegister(this);
        Destroy(gameObject);
    }

    protected void OnDestroy()
    {
        BoundsEnforcer.Destroy(gameObject);
    }
}
