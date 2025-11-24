using UnityEngine;
using UnityEngine.SceneManagement;

namespace Andrew
{
    public class TreeButtonBehaviour : MonoBehaviour
    {
        public void OnClick() {
            SceneManager.LoadScene(sceneName: "BoonTree"); 
        }
        
    }
}