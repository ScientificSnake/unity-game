using UnityEngine;

public class EscapeSavingPopup : MonoBehaviour
{
    public GameObject Popup;

    public void OnClick()
    {
        Popup.SetActive(false);
    }
}
