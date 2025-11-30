using UnityEngine;

public class ThrottleLineMover : MonoBehaviour
{
    public GameObject PlayerRef;
    public PlayerObjectScript PlayerScript;
    public Camera mainCamera;

    [SerializeField] float MaxX;
    [SerializeField] float MinX;

    private void Update()
    {
        if (PlayerRef == null)
        {
            PlayerRef = GameObject.FindWithTag("Player");
            PlayerScript = PlayerRef.GetComponent<PlayerObjectScript>();
        }
    }

    private void FixedUpdate()
    {
        float throttleProportion = PlayerScript.throttle / 85;

        float targetX = (MaxX - MinX) * throttleProportion + MinX;

        transform.position = mainCamera.ScreenToWorldPoint(new Vector3(targetX + 567.1f, 220-123.7f, 0));

        print($"Target position pre world {new Vector3(targetX, 0, 0)}");
        print($"Target positon post world {transform.position}");
        print($"target x is {targetX}");
    }
}
