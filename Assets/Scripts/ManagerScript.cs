using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public static ManagerScript Instance { get; private set; }
    public static int BoonCredits;
    public static int HighestLevelCompleted;

    #region Currect Boon configuration data
    public static List<string> UnlockedBoons = new List<string>();

    public static Dictionary<string, bool> IsBoonPurchased = new Dictionary<string, bool>  // THIS INCLUDES ALL THE BOONS AND WHETHER THEY HAVE BEEN PURCHASED OR NOT
    {
        { "TestBoon1", false}
    };

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoonCredits = 100;
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
