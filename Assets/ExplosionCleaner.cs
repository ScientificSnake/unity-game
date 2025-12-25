using System.Collections;
using UnityEngine;

public class ExplosionCleaner : MonoBehaviour
{
    void Start()
    {
        KillSelf();
    }

    private IEnumerator KillSelf()
    {
        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }
}
