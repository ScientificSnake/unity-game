using System.IO;
using UnityEngine;

public class RoundOverImage : MonoBehaviour
{
    public GameObject EndOfRoundButtons;

    public GameObject boon1Btn;
    public GameObject boon2Btn;
    public GameObject boon3Btn;

    public GameObject[] boonButtons;

    public void Start()
    {
        boonButtons = new GameObject[3] { boon1Btn, boon2Btn, boon3Btn };
    }
}
