using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public static ManagerScript Instance { get; private set; }
    public static int BoonCredits;
    public static int HighestLevelCompleted;

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
