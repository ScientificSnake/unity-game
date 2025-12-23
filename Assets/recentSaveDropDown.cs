using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class RecentSaveDropDown : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;
    public TMPro.TMP_InputField inputField;

    public void refreshOptions()
    {
        if (ManagerScript.Instance == null)
        {
            print("Manger script is null");
        }
        List<string> options = ManagerScript.Instance.RecentFiles.ToList();

        options.Reverse();

        options.Insert(0, "Choose a recent save");
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    public void ValueChangedLogic()
    {
        if (dropdown.value != 0)
        {
            inputField.text = dropdown.options[dropdown.value].text;
        }
    }
}
