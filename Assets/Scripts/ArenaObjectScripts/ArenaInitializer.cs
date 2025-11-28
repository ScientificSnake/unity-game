using UnityEngine;

public class ArenaInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ManagerScript.CurrentLevelManagerInstance.InstantiatePlayerObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
