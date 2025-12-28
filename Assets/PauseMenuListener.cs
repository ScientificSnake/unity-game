using UnityEngine;

public class PauseMenuListener : MonoBehaviour
{
    public InArenaControls inputManager;
    public GameObject PauseMenu;
    private void Start()
    {
        inputManager = new InArenaControls();

        inputManager.PauseMenu.Enable();

        inputManager.PauseMenu.Escape.performed += Escape_performed;
    }

    private void Escape_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("<color=red> ESCAPE DETECTED");

        if (MiniMapRegister.MiniMapShown)
        {
            MiniMapRegister.DisableMiniMap();
        }
        else 
        {
            if (PauseMenu.activeSelf == false)
            {
                Time.timeScale = 0;

                try
                {
                    GameObject PlayerObj = GameObject.FindWithTag("Player");
                    PlayerObjectScript PlayerScript = PlayerObj.GetComponent<PlayerObjectScript>();

                    PlayerScript.inputManager.Player.Disable();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e + " | Is player not here? Is round ending?");
                }

                CameraFollowScript camScript = Camera.main.GetComponent<CameraFollowScript>();
                camScript.inputManager.CameraControls.Disable();

                PauseMenu.SetActive(true);
            }

            else
            {
                Time.timeScale = 1;
                try
                {
                    GameObject PlayerObj = GameObject.FindWithTag("Player");
                    PlayerObjectScript PlayerScript = PlayerObj.GetComponent<PlayerObjectScript>();

                    PlayerScript.inputManager.Player.Enable();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e + " | Is player not here? Is round ending?");
                }

                CameraFollowScript camScript = Camera.main.GetComponent<CameraFollowScript>();
                camScript.inputManager.CameraControls.Enable();

                PauseMenu.SetActive(false);
            }
        }
    }
}
