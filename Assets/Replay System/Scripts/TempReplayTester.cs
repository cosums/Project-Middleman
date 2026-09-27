using UnityEngine;
using UnityEngine.UI;

public class TempReplayTester : MonoBehaviour
{
    public Button ReplayButton;
    public FixedTicker Ticker;
    public PlayerController Controller;

    public void OnEnable()
    {
        ReplayButton.onClick.AddListener(TriggerReplay);
    }

    public void OnDisable()
    {
        ReplayButton.onClick.RemoveListener(TriggerReplay);
    }

    public void TriggerReplay()
    {
        Controller.RestoreStart();
        Ticker.FinishAndReplay();
    }

    public void EndReplay()
    {
        
    }
}
