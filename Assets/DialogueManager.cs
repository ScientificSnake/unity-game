using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject _dialogueBox;
    [SerializeField] private GameObject _dialogueText;

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioSource auxsource;

    public InArenaControls inputManager;

    public static DialogueManager Instance;

    private void Awake()
    {
        Instance = this;
        inputManager = new InArenaControls();
        inputManager.Dialogue.EnterPressed.performed += EnterEvent;
    }

    private void EnterEvent(InputAction.CallbackContext obj)
    {
        auxsource.PlayOneShot(clickSound);
        _dialogueBox.SetActive(false);
        _dialogueText.SetActive(false);
        inputManager.Dialogue.Disable();
        Time.timeScale = 1;
        Camera.main.GetComponent<CameraFollowScript>().inputManager.CameraControls.Enable();
        GameObject.FindWithTag("Player").GetComponent<PlayerObjectScript>().inputManager.Player.Enable();
        GameObject.Find("PauseMenuListener").GetComponent<PauseMenuListener>().inputManager.PauseMenu.Enable();
    }

    public static void DisplayDialogue(string text, bool hard)
    {
        Instance._dialogueBox.SetActive(true);
        Instance._dialogueText.SetActive(true); // unity i dont trust you to properly do inheiretance

        if (hard)
        {
            Time.timeScale = 0;
            Camera.main.GetComponent<CameraFollowScript>().inputManager.CameraControls.Disable();
            GameObject.FindWithTag("Player").GetComponent<PlayerObjectScript>().inputManager.Player.Disable();
            GameObject.Find("PauseMenuListener").GetComponent<PauseMenuListener>().inputManager.PauseMenu.Disable();
        }

        Instance._dialogueText.GetComponent<TextMeshProUGUI>().text = text;
        
        Instance.inputManager.Dialogue.Enable();
    }
}
