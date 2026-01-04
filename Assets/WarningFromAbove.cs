using System;
using System.Collections;
using UnityEngine;

public class WarningFromAbove : MonoBehaviour
{
    private enum State
    {
        InWarning,
        Base
    }

    private State state;
    private float startY;


    private void Awake()
    {
        state = State.Base;
        startY = transform.position.y;
    }

    public void SendWarn()
    {
        if (state == State.Base)
        {
            state = State.InWarning;
            transform.LeanMoveY(1620, 0.7f).setEaseInCubic();
            StartCoroutine(putBack(1.25f));
        }
    }

    private IEnumerator putBack(float seconds)
    {
        print($"<color=yellow> putback pre triggerend for " + seconds);
        yield return new WaitForSeconds(seconds);
        print($"<color=yellow> putback post triggerend for " + seconds);
        transform.LeanMoveY(startY, 0.5f).setEaseOutCubic();
        state = State.Base;
    }
}
