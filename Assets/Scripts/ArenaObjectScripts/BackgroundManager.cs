using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private Vector2 initialOffset = new Vector2(-552.44355f, -176.6316f);

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
        PlayerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        print($"width is {width}, height is {height}");
    }

    private void FixedUpdate()
    {
        if (PlayerRb == null)
        {
            PlayerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        }
        Vector2 distance = (Vector2)MainCam.transform.position - (Vector2) transform.position;

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
        Vector2 fakevelo = PlayerRb.linearVelocity - 2 * (PlayerRb.linearVelocity.normalized * Mathf.Sqrt(PlayerRb.linearVelocity.magnitude));
        transform.transform.position += (Vector3) fakevelo * Time.fixedDeltaTime;
    }
}
