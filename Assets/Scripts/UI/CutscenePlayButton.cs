using UnityEngine;
using UnityEngine.UI;

public class CutscenePlayButton : MonoBehaviour
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
        GameManager.Instance.StartGame();
    }
}
