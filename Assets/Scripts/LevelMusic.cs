using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    public AudioClip levelMusic;
    public AudioClip replayAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayLevelAudio();
    }

    public void PlayLevelAudio()
    {
        if (GameManager.Instance.CurrentState == GameState.Replaying) {
            AudioBus.Instance.PlaySFX(replayAudio, 0.2f);
        } else {
            AudioBus.Instance.PlayMusic(levelMusic);
        }
        AudioBus.Instance.SetMusicVolume();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
