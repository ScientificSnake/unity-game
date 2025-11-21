using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    public static ManagerScript Instance { get; private set; }
    public static int HighestLevelCompleted;

    #region Currect Boon configuration data
    public static List<string> UnlockedBoons = new List<string>();
    public static int TechCredits;

    public static Dictionary<string, bool> NodePurchaseLog = new Dictionary<string, bool>  // THIS INCLUDES ALL THE Nodes AND WHETHER THEY HAVE BEEN PURCHASED OR NOT
    {
        { "TestBoon1", false},
        { "TestNode2", false}
    };

    public bool IsNodePurchased(string sysname)
    {
        return NodePurchaseLog[sysname];
    }
    public void PurchaseNode(string sysname)
    {
        NodePurchaseLog[sysname] = true;
    }


    public bool AreNodeDependciesMet(List<string> Dependencies)
    {
        foreach (string Dependency in Dependencies)
        {
            if ((IsNodePurchased(Dependency) is false))
            {
                return false;
            }
        }
        return true;
    }


    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TechCredits = 100;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
