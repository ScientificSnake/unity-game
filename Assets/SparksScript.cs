using System.Collections;
using UnityEngine;

public class SparksScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyInSeconds(1));
    }

    private IEnumerator DestroyInSeconds(int t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
}
