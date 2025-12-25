using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.U2D.IK;

public class BackgroundManager : MonoBehaviour
{
    public GameObject PauseMenuRef;

    [SerializeField] private Vector2 FakeVeloOffset;

    [SerializeField] private GameObject C;
    [SerializeField] private GameObject L;
    [SerializeField] private GameObject R;
    [SerializeField] private GameObject TC;
    [SerializeField] private GameObject BC;
    [SerializeField] private GameObject TR;
    [SerializeField] private GameObject BR;
    [SerializeField] private GameObject BL;
    [SerializeField] private GameObject TL;

    [SerializeField] private GameObject MainCam;
    [SerializeField] private Rigidbody2D PlayerRb;

    private float width; 
    private float height;

    private void ShiftUp()
    {
        float oldx;
        float oldy;
        print("shifting up");
        foreach (GameObject ninth in new GameObject[] {BL, BC, BR })
        {
            oldx = ninth.transform.position.x;
            oldy = ninth.transform.position.y;

            ninth.transform.position = new Vector2(oldx, oldy + height);
        }
        oldx = transform.position.x;
        oldy = transform.position.y;
        transform.position = new Vector2(oldx, oldy + height);
    }

    private void ShiftDown()
    {
        float oldx;
        float oldy;
        print("shifting down");
        foreach (GameObject ninth in new GameObject[] { TL, TC, TR })
        {
            oldx = ninth.transform.position.x;
            oldy = ninth.transform.position.y;

            ninth.transform.position = new Vector2(oldx, oldy - height);
        }
        oldx = transform.position.x;
        oldy = transform.position.y;
        transform.position = new Vector2(oldx, oldy - height);
    }

    private void ShiftLeft()
    {
        float oldx;
        float oldy;
        print("shifting left");
        foreach (GameObject ninth in new GameObject[] { TR, R, BR })
        {
            oldx = ninth.transform.position.x;
            oldy = ninth.transform.position.y;

            ninth.transform.position = new Vector2(oldx - width, oldy);
        }
        oldx = transform.position.x;
        oldy = transform.position.y;
        transform.position = new Vector2(oldx - width, oldy);
    }

    private void ShiftRight()
    {
        print("shifting right");
        float oldx;
        float oldy;
        foreach (GameObject ninth in new GameObject[] { TL, L, BL })
        {
            oldx = ninth.transform.position.x;
            oldy = ninth.transform.position.y;

            ninth.transform.position = new Vector2(oldx + width, oldy);
        }
        oldx = transform.position.x;
        oldy = transform.position.y;
        transform.position = new Vector2(oldx + width, oldy);
    }

    private void Awake()
    {
        width = C.GetComponent<SpriteRenderer>().bounds.size.x;
        height = C.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void Start()
    {
        FakeVeloOffset = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (PlayerRb == null)
        {
            PlayerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        }

        // Calculate background velocity as sqrt of player velocity magnitude
        // This makes background move slower and caps the speed growth
        float speedFactor = Mathf.Sqrt(PlayerRb.linearVelocity.magnitude);
        Vector2 backgroundVelocity = PlayerRb.linearVelocity.normalized * speedFactor;

        // Update the fake velocity offset
        FakeVeloOffset += backgroundVelocity * Time.fixedDeltaTime;

        //print($"Fake velo offset is {FakeVeloOffset.ToString()}");

        // Position background based on player position + offset
        transform.position = (Vector2)PlayerRb.gameObject.transform.position - FakeVeloOffset;

        // Check for shifting
        Vector2 distance = (Vector2)MainCam.transform.position - (Vector2)transform.position;

        if (distance.x > width)
        {
            ShiftRight();
        }
        if (distance.y > height)
        {
            ShiftUp();
        }
        if (distance.x < -width)
        {
            ShiftLeft();
        }
        if (distance.y < -height)
        {
            ShiftDown();
        }
    }
}
