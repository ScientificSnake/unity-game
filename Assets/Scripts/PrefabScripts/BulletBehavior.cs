using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletBehavior : MonoBehaviour
{
    public float Damage;
    public Rigidbody2D rb;
    public CapsuleCollider2D collider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        collider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.gravityScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject OtherGO = collision.gameObject;

        if (OtherGO.CompareTag("BasicSpacePirate"))
        {
            BasicPirateDummyBehaviour Otherscript = OtherGO.GetComponent<BasicPirateDummyBehaviour>();

            Otherscript.ApplyDamage(Damage);
        }
        // destroy self
        Destroy(gameObject);
    }

    public void ActivateCollider()
    {
        collider.enabled = true;
    }
}
//haisdfgihhsbdfoyig