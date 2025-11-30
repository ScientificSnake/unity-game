using System.Collections;
using UnityEngine;

public class BasicPirateDummyBehaviour : MonoBehaviour
{
    public float Health;
    public Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Health = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float damage)
    {
        Debug.Log("Enemy -> YES A HITT");

        Health -= damage;
        
        if (Health <= 0)
        {
            DestructSelf();
        }
    }

    private void DestructSelf()
    {
        rb.angularVelocity = 3000;
        StartCoroutine(DestroyInSeconds(0.5f));
    }

    private IEnumerator DestroyInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}