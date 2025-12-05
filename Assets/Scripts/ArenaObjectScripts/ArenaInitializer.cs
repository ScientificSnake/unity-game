using System.Linq.Expressions;
using UnityEngine;

public class ArenaInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ManagerScript.Instantiate(ManagerScript.CurrentLevelManagerInstance.RootLevelData.LayoutPrefab);
        ManagerScript.CurrentLevelManagerInstance.StartRoundRoutine();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
