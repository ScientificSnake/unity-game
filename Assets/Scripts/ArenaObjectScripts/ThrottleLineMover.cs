using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class ThrottleLineMover : MonoBehaviour
{
    public GameObject PlayerRef;
    public PlayerObjectScript PlayerScript;
    public Camera mainCamera;

    Vector3 OriginalPosition;
    public float ParentWidth;

    private void Awake()
    {
        OriginalPosition = transform.localPosition;
        GameObject ParentObj = transform.parent.gameObject;
        ParentWidth = ParentObj.GetComponent<RectTransform>().sizeDelta.x;
    }


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
        float throttleProportion = (PlayerScript.throttle /100);
        if (throttleProportion < 0)
        {
            throttleProportion = 0;
        }

        float targetX = (ParentWidth) * throttleProportion;
        transform.localPosition = OriginalPosition + new Vector3(targetX, 0, 0);
    }
}
