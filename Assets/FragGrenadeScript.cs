using Sebastian;
using System.Collections;
using UnityEngine;

public class FragGrenadeScript : ProjectileTemplate
{
    public float FuseSeconds;
    private float BaseBlinkTimeWait = 1f;
    private float MinBlinkTimeWait = 0.001f;
    private float blinkTime = 0.05f;

    [SerializeField] private Sprite FlashingSprite;
    private Sprite RegularSprite;

    private float LastBlink;
    private float StartTime;
    protected SpriteRenderer spriteRenderer;

    protected WeaponryData.WeaponParameters ActiveWeaponParams;

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
        float curblinktime = Mathf.Max(MinBlinkTimeWait, BaseBlinkTimeWait * blinkWaitTimeMult);
        float timeSinceLastBlink = now - LastBlink;

        if (timeSinceLastBlink > curblinktime)
        {
            StartCoroutine(SingleBlinkCR());
        }
    }
    protected IEnumerator SingleBlinkCR()
    {
        spriteRenderer.sprite = FlashingSprite;
        LastBlink = Time.time;
        yield return new WaitForSeconds(blinkTime);
        spriteRenderer.sprite = RegularSprite;
        
    }

    protected void Activate()
    {
        // boom boom code go here
        WeaponryData.Weapon FragWeapon = WeaponryData.WeaponDict[6];

        ActiveWeaponParams.SpawnPos = transform.position;

        FragWeapon.SpawnPrefab(ActiveWeaponParams);

        Destroy(gameObject);
    }

    protected void Start()
    {
        ActiveWeaponParams = WeaponryData.WeaponDict[6].BaseWeaponParams;
        ActiveWeaponParams.IgnoredColliders = new()
        {
            tcollider
        };
        spriteRenderer = GetComponent<SpriteRenderer>();
        RegularSprite = spriteRenderer.sprite;
        StartTime = Time.time;
        LastBlink = Time.time;
    }
}
