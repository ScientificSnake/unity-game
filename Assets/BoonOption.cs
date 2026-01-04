using Mono.Cecil.Cil;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;

public class BoonOption : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public BoonData.BoonBuff boonRepresentative;
    public static TextMeshProUGUI descriptionTMP;
    public static TextMeshProUGUI titleTMP;
    public static NewRoundBtn newRoundBtn;
    

    public void OnClicK()
    {
        if (boonRepresentative != null)
        {
            descriptionTMP.text = boonRepresentative.Description;
            titleTMP.text = boonRepresentative.DisplayName;
            newRoundBtn.BoonAction = BoonData.GetBoonEffect(boonRepresentative);
        }
    }

    public void Awake()
    {
        if (descriptionTMP == null)
        {
            descriptionTMP = GameObject.Find("Boon Description").GetComponent<TextMeshProUGUI>();
        }
        if (titleTMP == null)
        {
            titleTMP = GameObject.Find("Boon Title").GetComponent<TextMeshProUGUI>();
        }
        if (newRoundBtn == null)
        {
            newRoundBtn = GameObject.Find("NextRoundBtn").GetComponent<NewRoundBtn>();
        }
    }
}
