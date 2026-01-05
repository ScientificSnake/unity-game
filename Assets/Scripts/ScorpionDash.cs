using System.Collections;
using UnityEngine;

public class ScorpionDash : MonoBehaviour
{
    public float DashForce = 800;
    public float DashTime = 0.15f;

    private float _ogDamping;

    private Rigidbody2D rb;
    private Coroutine currentUnDash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Dash()
    {
        _ogDamping = rb.linearDamping;
        rb.linearDamping = 0;
        Vector2 Force = transform.right * DashForce;
        rb.AddForce(Force, ForceMode2D.Impulse);
        currentUnDash = StartCoroutine(UnDash(transform.right));
    }

    private IEnumerator UnDash(Vector2 direction)
    {
        yield return new WaitForSeconds(DashTime);
        Vector2 Force = -1 * direction * DashForce;
        rb.AddForce(Force, ForceMode2D.Impulse);
        currentUnDash = null;
        rb.linearDamping = _ogDamping;
    }
}
