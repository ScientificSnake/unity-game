using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class LoadingScreenTextRotater : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public bool _shouldRepeat = true;

    private int tipIndex;

    private readonly string[] LoadingTips = {
        "Stealing Nearest Power Source",
        "Doing something on the Calc (short for calculator)"
    };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("starting loading screen");
        tipIndex = 0;
        SetDisplayText(LoadingTips[tipIndex]);
        StartCoroutine(textRotater());
    }

    IEnumerator textRotater()
    {
        while (_shouldRepeat)
        {
            yield return new WaitForSeconds(3);
            tipIndex++;
            if (tipIndex == LoadingTips.Length)
            {
                tipIndex = 0;
            }

            SetDisplayText(LoadingTips[tipIndex]);
        }
    }


    private void SetDisplayText(string text)
    {
        textMeshProUGUI.text = text;
    }
}