using UnityEngine;

public class OnDestroyDialogue : MonoBehaviour
{
    [SerializeField] private string DialogueText;

    [SerializeField] private bool HardFreeze;

    private void OnDestroy()
    {
        DialogueManager.DisplayDialogue(DialogueText, HardFreeze);
    }
}
