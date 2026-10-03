using Unity.VisualScripting;
using UnityEngine;

public class FilmManager : MonoBehaviour
{
    public GameObject FilmCamera;
    public GameObject MainCamera;

    [Header("Audio")]

    public AudioClip trainHorn;
    public AudioClip trainRunning;
    public AudioClip whistle;
    public ParticleSystem confetti;
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
            DialogueLine line = DialogueManager.Instance.NewDialogueLine("Director", "Cut! That's a wrap!", whistle);
            DialogueManager.Instance.EnqueueDialogue(line);
            ParticleSystem confettiParticles = Instantiate(confetti);
            confetti.transform.position = MainCamera.transform.position;
            confetti.transform.parent = MainCamera.transform;
        } 
        else if (GameManager.Instance.CurrentState == GameState.Replaying)
        {
            GameManager.Instance.FinishReplay();
        }
    }

    public void WinState()
    {
        GameManager.Instance.WinGame();
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
