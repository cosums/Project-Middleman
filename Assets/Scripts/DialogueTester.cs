using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTester : MonoBehaviour
{
    public string speaker1;
    public string content1;
    public float length1;

    public string speaker2;
    public string content2;
    public float length2;

    public string speaker3;
    public string content3;
    public float length3;

    private DialogueLine line1 = new DialogueLine();
    private DialogueLine line2 = new DialogueLine();
    private DialogueLine line3 = new DialogueLine();

    private InputFrame _currentInputFrame;

    public void Start()
    {
        line1.Init(speaker1, content1, length1);
        line2.Init(speaker2, content2, length2);
        line3.Init(speaker3, content3, length3);
    }

    public void SimulateTick(InputFrame frame)
    {
        _currentInputFrame = frame;

        if (_currentInputFrame.JumpWasPressed)
        {
            DialogueManager.Instance.EnqueueDialogue(line1);
        }
        if (_currentInputFrame.SlideWasPressed)
        {
            DialogueManager.Instance.EnqueueDialogue(line2);
        }
        if (_currentInputFrame.AttackWasPressed)
        {
            DialogueManager.Instance.EnqueueDialogue(line3);
        }
    }
}
