using UnityEngine;

public class TreeUpdates : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ManagerScript.Instance.UpdateNodes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
