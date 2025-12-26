using UnityEngine;

public class GameOverAdditionsScript : MonoBehaviour
{
    public AudioClip LoserSound;
    private AudioSource AuSource;
    void Start()
    {
        AuSource = GetComponent<AudioSource>();
        AuSource.PlayOneShot(LoserSound, 1);
    }
}
