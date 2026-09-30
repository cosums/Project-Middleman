using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioClip menuMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioBus.Instance.PlayMusic(menuMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
