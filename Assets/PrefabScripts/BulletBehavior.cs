using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BulletBehavior : MonoBehaviour
{
    public float Damage;
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.gravityScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("BULLET -> YES A HITT");

        GameObject OtherGO = collision.gameObject;

        if (OtherGO.CompareTag("BasicSpacePirate"))
        {
            print("Bullet -> we got a pirate ");

            BasicPirateDummyBehaviour Otherscript = OtherGO.GetComponent<BasicPirateDummyBehaviour>();

            Otherscript.ApplyDamage(Damage);
            // destroy self
            Destroy(gameObject);
        }
    }
}
//haisdfgihhsbdfoyig