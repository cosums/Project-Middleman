using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    public AudioClip levelMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioBus.Instance.PlayMusic(levelMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
