using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { MainMenu, Playing, Paused }
    public GameState CurrentState { get; private set; }

    public static event Action OnGameQuit;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Autoload()
    {
        GameObject autoObj = new GameObject("Game Manager");
        autoObj.AddComponent<GameManager>();

        Instance = autoObj.GetComponent<GameManager>();
        DontDestroyOnLoad(autoObj);
    }

    public void ChangeState(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Playing:
                break;
            case GameState.Paused:
                break;
        }
    }

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        OnGameQuit?.Invoke();
        Application.Quit();
    }
    
}
