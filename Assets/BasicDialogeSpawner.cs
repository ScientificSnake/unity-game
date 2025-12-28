using System.Collections;
using UnityEngine;

public class BasicDialogeSpawner : MonoBehaviour
{
    [SerializeField] private float _WaitTime;
    [SerializeField] private string Text;
    [SerializeField] private bool HardFreeze;

    private void Start()
    {
        StartCoroutine(DialogueInSeconds(_WaitTime, Text, HardFreeze));
    }

    private IEnumerator DialogueInSeconds(float seconds, string text, bool hard)
    {
        yield return new WaitForSeconds(seconds);
        DialogueManager.DisplayDialogue(text, hard);
    }
}
