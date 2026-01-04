using System.Collections;
using UnityEngine;

public class ScorpionDash : MonoBehaviour
{
    public float DashForce;
    public float DashTime;

    private Rigidbody2D rb;
    private Coroutine currentUnDash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Dash()
    {
        Vector2 Force = transform.eulerAngles.normalized * DashForce;
        rb.AddForce(Force, ForceMode2D.Impulse);
        currentUnDash = StartCoroutine(UnDash());
    }

    private IEnumerator UnDash()
    {
        yield return new WaitForSeconds(DashTime);

        Vector2 Force = -1 * transform.eulerAngles.normalized * DashForce;
        rb.AddForce(Force, ForceMode2D.Impulse);
        currentUnDash = null;
    }
}
