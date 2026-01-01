using UnityEngine;

public class EscapeSavingPopup : MonoBehaviour
{
    public GameObject Popup;

    public void OnClick()
    {
        Popup.LeanScale(Vector3.zero, 1);
    }
}
