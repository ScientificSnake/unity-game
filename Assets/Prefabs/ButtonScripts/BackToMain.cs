using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BackToMainScript()
    {
        SceneManager.LoadScene(sceneName: "mainmenu");
    }
}
