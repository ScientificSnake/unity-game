using System.Collections;
using System.Globalization;
using UnityEngine;

public class RoundButtonsAnimatedDropScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        print($"<color=orange> Droping down");
        StartCoroutine(dropDown(2));
    }

    private IEnumerator dropDown(float seconds)
    {
        print($"<color=yellow> dropping down the scren");
        yield return new WaitForSeconds(seconds);
        transform.LeanMoveLocalY(0, 1).setEaseOutElastic();
    }
}
