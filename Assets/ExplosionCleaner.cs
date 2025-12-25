using System.Collections;
using UnityEngine;

public class ExplosionCleaner : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(KillSelf());
    }

    private IEnumerator KillSelf()
    {
        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }
}
