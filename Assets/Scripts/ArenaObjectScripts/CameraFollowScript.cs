using JetBrains.Annotations;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowScript : MonoBehaviour
{
    private float CameraCircleRad = 50f;
    private float LightCenteringRate;
    private float TrueDeadZone = 0.05f;

    public float maxCamZoom = 300;
    public float minCamZoom = 100;


    public PlayerObjectScript PlayerReference;

    private Camera cam;

    public InArenaControls inputManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();

        GameObject PlayerGO = GameObject.FindWithTag("Player");
        PlayerReference = PlayerGO.GetComponent<PlayerObjectScript>();
    }

    private void MoveX(float distance, bool useDeltaTime=false)
    {
        if (useDeltaTime)
        {
            transform.position += new Vector3(distance, 0, 0) * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(distance, 0, 0);
        }
    }

    private void MoveY(float distance, bool useDeltaTime=false)
    {
        if (useDeltaTime)
        {
            transform.position += new Vector3(0, distance, 0) * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(0, distance, 0);
        }

    }


    // Update is called once per frame
    void LateUpdate()
    {

        // find magnitide

        float xDist = Mathf.Abs(transform.position.x - (PlayerReference.transform.position.x));
        float yDist = Mathf.Abs(transform.position.y - (PlayerReference.transform.position.y));
        float magnitude = Mathf.Sqrt(xDist * xDist + yDist * yDist);

        //print($"X dist -> ({xDist}), Y dist -> ({yDist}), magnitude -> ({magnitude})");

        LightCenteringRate = magnitude*0.75f;//distance camera moves to center per frame set to a quarter of the magnitude of camera distance from player distance

        float directedXDist = PlayerReference.transform.position.x - transform.position.x;
        float directedYDist = PlayerReference.gameObject.transform.position.y - transform.position.y;


        // scale down the x and y so the magnitude is 100
        if (magnitude > CameraCircleRad)
        {
            float scaleFactor = 1 - (CameraCircleRad / magnitude);

            MoveX(directedXDist * scaleFactor);
            MoveY(directedYDist * scaleFactor);

        }

        // light centering force
        if (magnitude > TrueDeadZone)
        {
            float centeringFactor = (LightCenteringRate / magnitude);
            MoveX(centeringFactor * directedXDist, true);
            MoveY(centeringFactor * directedYDist, true);

            //print($"Centering factor is {centeringFactor}");
            //print($"Magnitude is {magnitude}");
            //print($"Light centering foce rate is {LightCenteringRate}");

            //print($"Moving x by {centeringFactor * directedXDist}");
        }
    }

    private void OnEnable()
    {
        inputManager = new InArenaControls();

        inputManager.CameraControls.Scroll.performed += Scroll_performed;

        inputManager.Enable();
    }

    private void Scroll_performed(InputAction.CallbackContext context)
    {
        Vector2 scrollDelta = context.ReadValue<Vector2>();

        float camzoom = cam.orthographicSize;

        if (scrollDelta.y > 0)
        {
            camzoom -= 5;
        }
        else if (scrollDelta.y < 0)
        {
            camzoom += 5;
        }

        camzoom = Mathf.Clamp(camzoom, minCamZoom, maxCamZoom);

        cam.orthographicSize = camzoom;
    }
}
