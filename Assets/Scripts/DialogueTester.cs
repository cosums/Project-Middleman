using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTester : MonoBehaviour
{

    private DialogueLine line1 = new DialogueLine("Director", "Fuck you", 1f);
    private DialogueLine line2 = new DialogueLine("Director", "kys", 1f);
    private DialogueLine line3 = new DialogueLine("Director", "idk man", 1f);

    private InputFrame _currentInputFrame;

    public void SimulateTick(InputFrame frame)
    {
        _currentInputFrame = frame;

        if (_currentInputFrame.AttackWasPressed)
        {
            QuicktimeManager.Instance.AddQuickTimeEvent();
        }
        
    }
}
