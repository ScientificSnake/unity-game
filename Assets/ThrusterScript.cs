using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ThrusterScript : MonoBehaviour
{
    public Light2D thrusterLight;
    private readonly float baseLightIntensity = 0.9f;
    private readonly float minLightIntensity = 0.3f;

    public void SetScale(float throttle, Vector2 baseScale)
    {
        Vector2 targetScale = baseScale * (throttle / 100);
        transform.localScale = targetScale;

        thrusterLight.intensity = Mathf.Max(minLightIntensity, (throttle/100)  * baseLightIntensity);
    }
}
