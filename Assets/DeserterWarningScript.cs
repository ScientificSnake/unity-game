using System.Collections;
using TMPro;
using UnityEngine;

public class DeserterWarningScript : MonoBehaviour
{
    [SerializeField] private int TimerSeconds;

    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private Coroutine Timer;

    public void StartTimer()
    {
        if (Timer == null && !(gameObject.activeSelf))
        {
            gameObject.SetActive(true);
            Timer = StartCoroutine(TimerRoutine());
        }
    }

    private IEnumerator TimerRoutine()
    {
        for (int i = TimerSeconds;  i >= 1; i--)
        {
            timerText.text = $"Detonating charges in {i}";
            yield return new WaitForSeconds(1);
        }
        ManagerScript.CurrentLevelManagerInstance.GameOver();
        Timer = null;
    }

    public void StopTimer()
    {
        StopCoroutine(Timer);
        gameObject.SetActive(false);
    }
}
