using System.Collections;
using TMPro;
using UnityEngine;

public class DeserterWarningScript : MonoBehaviour
{
    private int TimerSeconds = 15;

    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        TimerSeconds = 15;
        gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        if (!(gameObject.activeSelf))
        {
            gameObject.SetActive(true);
            StartCoroutine(TimerRoutine());
        }
    }

    private IEnumerator TimerRoutine()
    {
        int second = TimerSeconds;

        while (true)
        {
            timerText.text = $"Detonating charges in {second}";
            yield return new WaitForSeconds(1);
            second--;
            if (second == 0)
            {
                break;
            }
        }
        ManagerScript.CurrentLevelManagerInstance.GameOver();
    }

    public void StopTimer()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
