using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioClip menuMusic;
    public AudioClip startupSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioBus.Instance.PlayMusic(menuMusic);
        // AudioBus.Instance.PlaySFX(startupSound, 1.0f, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
