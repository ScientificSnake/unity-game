using UnityEngine;

public class AutoHider : MonoBehaviour
{
    public void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Color color = spriteRenderer.color;

        color.a = 0;
        spriteRenderer.color = color;
        //disable the sprite renderer
    }
}
