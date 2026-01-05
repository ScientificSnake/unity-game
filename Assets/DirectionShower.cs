using UnityEngine;
public class DirectionShower : MonoBehaviour
{
    public PlayerObjectScript playerscriptref;

    public Vector2 baseScale;

    [SerializeField] private float minimumvelo = 40;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        Vector2 normalizedDir = playerscriptref.rb.linearVelocity;

        transform.eulerAngles = new Vector3(0, 0, normalizedDir.DirectionAngle());

        if (playerscriptref.rb.linearVelocity.magnitude < minimumvelo)
        {
            transform.localScale = Vector3.zero;
        }
        else
        {
            transform.localScale = baseScale * (Mathf.Log(playerscriptref.rb.linearVelocity.magnitude));
        }
    }
}
