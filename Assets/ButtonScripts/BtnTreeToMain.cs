using UnityEngine;
using UnityEngine.SceneManagement;

    public class BackToMainButtonBehaviour : MonoBehaviour
    {
        public void OnClick()
        {
            Debug.Log(message: "Back button clicked by user");
            SceneManager.LoadScene(sceneName: "mainmenu");

        }

    }