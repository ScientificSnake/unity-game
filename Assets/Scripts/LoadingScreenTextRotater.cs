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

    private string[] LoadingTips = {
        "Stealing Nearest Power Source",
        "Doing something on the Calc (short for calculator)",
        "Digging in my butt"
    };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        ShufflerNonMono.Shuffle(LoadingTips);

        Debug.Log("starting loading screen");
        tipIndex = UnityEngine.Random.Range(0, LoadingTips.Length);
        SetDisplayText(LoadingTips[tipIndex]);
        StartCoroutine(TextRotater());
    }

    IEnumerator TextRotater()
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