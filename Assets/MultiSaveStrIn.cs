using TMPro;
using UnityEngine;

public class MultiSaveStrIn : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    public string targetFileName;

    public void StoreFileName()
    {
        targetFileName = inputField.text;
    }
}
