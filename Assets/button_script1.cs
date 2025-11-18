using UnityEngine;

namespace Andrew
{
    public class TreeButtonBehaviour : MonoBehaviour
    {
        public void TestClick()
        {
            Debug.Log(message: "didsomething");
        }

        public void OnClick() {
            Debug.Log(message: "Tree button clicked by user");
        }
        
    }
}