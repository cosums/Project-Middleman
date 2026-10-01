using UnityEngine;

public class QuicktimeManager : MonoBehaviour
{
    public AudioClip quicktimeMusic;

    private DialogueLine line1;
    private DialogueLine line2;
    private DialogueLine line3;
    private DialogueLine line4;

    public static QuicktimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        line1 = new DialogueLine("Director", "Strick a pose in 3...", 1f);
        line2 = new DialogueLine("Director", "2...", 1f);
        line3 = new DialogueLine("Director", "1...", 1f);
        line4 = new DialogueLine("Director", "Now!!", 1f, true);

    }

    public void AddQuickTimeEvent()
    {
        DialogueManager.Instance.EnqueueDialogue(line1);
        DialogueManager.Instance.EnqueueDialogue(line2);
        DialogueManager.Instance.EnqueueDialogue(line3);
        DialogueManager.Instance.EnqueueDialogue(line4);
    }

}
