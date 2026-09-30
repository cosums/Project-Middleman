using UnityEngine;
using UnityEngine.UI;

public class TempReplayTester : MonoBehaviour
{
    public Button ReplayButton;

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
        GameManager.Instance.PlayReplay();
    }

    public void EndReplay()
    {
        
    }
}
