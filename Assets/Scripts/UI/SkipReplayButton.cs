using UnityEngine;
using UnityEngine.UI;

public class SkipReplayButton : MonoBehaviour
{
    public Button MenuButton;

    public void OnEnable()
    {
        MenuButton.onClick.AddListener(PlayGame);
    }

    public void OnDisable()
    {
        MenuButton.onClick.RemoveListener(PlayGame);
    }

    public void PlayGame()
    {
        GameManager.Instance.FinishReplay();
    }
}
