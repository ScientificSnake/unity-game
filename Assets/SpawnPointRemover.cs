using UnityEngine;

public class SpawnPointRemover : AutoHider
{
    public void Start()
    {
        GameObject.FindWithTag("Player").transform.position = transform.position;
    }
}
