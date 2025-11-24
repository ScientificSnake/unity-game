using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMapScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnClick()
    {
        SceneManager.LoadScene(sceneName: "MapScreen");
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
