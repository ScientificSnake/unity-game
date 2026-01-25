using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTemplate : MonoBehaviour, IBoundsCheckable
{
    public Rigidbody2D rb;
    public Rigidbody2D Rigidbody2 => rb;
    public Collider2D tcollider;
    public List<Action<Collision2D>> OnHitExecutions;

    protected void ApplyHitFuncs(Collision2D collision)
    {
        for (int i = 0; i < OnHitExecutions.Count; i++)
        {
            OnHitExecutions[i](collision);
        }
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tcollider = GetComponent<Collider2D>();
        BoundsEnforcer.Register(this);
        rb.linearDamping = 0.001f;
        rb.angularDamping = 0f;
        OnHitExecutions = new();
        rb.gravityScale = 0f;
    }

    public void OnOutOfBounds()
    {
        BoundsEnforcer.DeRegister(this);
        Destroy(gameObject);
    }

    protected void OnDestroy()
    {
        BoundsEnforcer.DeRegister(this);
    }
}
