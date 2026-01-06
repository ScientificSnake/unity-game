using UnityEngine;

public class OnDestroyDialogue : MonoBehaviour
{
    [SerializeField] private string DialogueText;

    [SerializeField] private bool HardFreeze;

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            if (ManagerScript.CurrentLevelManagerInstance.InRound)
                DialogueManager.DisplayDialogue(DialogueText, HardFreeze);
        }
    }
}
