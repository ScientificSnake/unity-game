using System.Linq.Expressions;
using UnityEngine;

public class ArenaInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public DeserterWarningScript deserterWarning;

    void Start()
    {
        print($"Arena start initilizer starting round");
        ManagerScript.CurrentLevelManagerInstance.StartRoundRoutine();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
