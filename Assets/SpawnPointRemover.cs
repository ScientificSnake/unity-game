using UnityEngine;

public class SpawnPointRemover : MonoBehaviour
{
    public void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Color color = spriteRenderer.color;

        color.a = 0;
        spriteRenderer.color = color;
        //disable the sprite renderer
    }

    public void Start()
    {
        GameObject.FindWithTag("Player").transform.position = transform.position;
    }
}
