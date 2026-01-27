using UnityEngine;

public class FragGrenadeScript : ProjectileTemplate
{
    public float FuseSeconds;
    private float BaseBlinkTimeWait;
    private float MinBlinkTimeWait;

    private float LastBlink;

    private float StartTime;

    protected SpriteRenderer spriteRenderer;

    protected void Update()
    {
        float now = Time.time;
        float timePassed = now - StartTime;

        if (timePassed >= FuseSeconds)
        {
            Activate();
            return;
        }

        float timeLeftPortion = 1 - (timePassed / FuseSeconds);
        float blinkWaitTimeMult = Mathf.Pow(timeLeftPortion, 0.333f);
        float curblinktime = Mathf.Min(MinBlinkTimeWait, BaseBlinkTimeWait * blinkWaitTimeMult);
        float timeSinceLastBlink = now - LastBlink;

        if (timeSinceLastBlink > curblinktime)
        {
            Blink();    
        }
    }

    protected void Blink()
    {
        Color color = spriteRenderer.color;
        color.r = 1;
        color.g = 1;
        color.b = 1;
        spriteRenderer.color = color;
        LastBlink = Time.time;
    }

    protected void Activate()
    {
        // boom boom code go here
    }

    protected void Start()
    {
        StartTime = Time.time;
        LastBlink = Time.time;
    }
}
