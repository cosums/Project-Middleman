using Unity.VisualScripting;
using UnityEngine;

public class FilmManager : MonoBehaviour
{
    public GameObject FilmCamera;

    [Header("Audio")]

    public AudioClip trainHorn;
    public AudioClip trainRunning;
    public Train train;
    
    public void Awake()
    {
        if (GameManager.Instance.CurrentState == GameState.Replaying)
        {
            FilmCamera.SetActive(true);
        }
    }

    public void FinishReplay()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.WinGame();
        } 
        else if (GameManager.Instance.CurrentState == GameState.Replaying)
        {
            GameManager.Instance.FinishReplay();
        }
    }

    public void QuickTimeEvent()
    {
        QuicktimeManager.Instance.AddQuickTimeEvent();
    }

    public void PlayTrainHorn()
    {
        AudioBus.Instance.PlaySFX(trainHorn);
    }

    public void PlayTrainRunning()
    {
        AudioBus.Instance.PlaySFX(trainRunning);
    }

    public void StartTrain()
    {
        train.StartTrain();
    }
}
