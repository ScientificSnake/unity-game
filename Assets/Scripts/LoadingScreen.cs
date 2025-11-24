using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public GameObject targetGameObject;

    public static LoadingScreen Instance { get; private set; }
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

    public void Start()
    {
        targetGameObject.SetActive(false);
    }

    public void Enable()
    {
        targetGameObject.SetActive(true);
    }

    public void Disable()
    {
        targetGameObject.SetActive(false);
    }
}
