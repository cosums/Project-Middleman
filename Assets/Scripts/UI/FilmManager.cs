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
            ParticleSystem confettiParticles = Instantiate(confetti);
            Vector3 pos = MainCamera.transform.position;
            confettiParticles.transform.position = new Vector3(pos.x, pos.y - 17, 0);
            DialogueLine line = DialogueManager.Instance.NewDialogueLine("Director", "Cut! That's a wrap!", whistle);
            DialogueManager.Instance.EnqueueDialogue(line);
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
