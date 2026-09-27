using UnityEngine;
using UnityEngine.UI;

public class MenuQuitButton : MonoBehaviour
{
    Button MenuButton;
    
    public void Awake()
    {
        MenuButton = GetComponent<Button>();
    }

    public void OnEnable()
    {
        MenuButton.onClick.AddListener(TriggerQuit);
    }

    public void OnDisable()
    {
        MenuButton.onClick.RemoveListener(TriggerQuit);
    }

    private void TriggerQuit()
    {
        GameManager.Instance.QuitGame();
    }
}
