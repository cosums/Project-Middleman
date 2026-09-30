using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Replaying, Paused, Failed, Won, Finished }
public enum DeathCondition { Death, OutOfFrame }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; } = GameState.Menu;

    public static event Action<GameState> OnNewState;
    public static event Action OnGameQuit;

    public DeathCondition LastDeathReason;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Autoload()
    {
        GameObject autoObj = new GameObject("Game Manager");
        autoObj.AddComponent<GameManager>();

        Instance = autoObj.GetComponent<GameManager>();
        DontDestroyOnLoad(autoObj);

        if (SceneManager.GetActiveScene().name == "Level")
        {
            Instance.CurrentState = GameState.Playing;
        }
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log($"[GameManager]: Switching to game state {newState}");
        UpdateState(newState);
        CurrentState = newState;
        OnNewState?.Invoke(newState);
    }

    private void UpdateState(GameState state) {
        switch (state)
        {
            case GameState.Menu:
                break;
            case GameState.Playing:
                break;
            case GameState.Replaying:
                break;
            case GameState.Failed:
                break;
            case GameState.Won:
                break;
            case GameState.Paused:
                break;
        }
    }

    public void StartGame()
    {
        ChangeState(GameState.Playing);
        LoadScene("Level");
    }

    public void Pause()
    {
        ChangeState(GameState.Paused);
    }

    public void LoseGame(DeathCondition deathReason)
    {
        LastDeathReason = deathReason;
        //ChangeState(GameState.Failed);
    }

    public void ReturnToMenu()
    {
        LoadScene("Menu");
        ChangeState(GameState.Menu);
    }

    public void WinGame()
    {
        ChangeState(GameState.Won);
        PlayReplay(); // CHANGE THIS LATER
    }

    public void PlayReplay()
    {
        ChangeState(GameState.Replaying);
        LoadScene("Level");
    }

    public void FinishReplay()
    {
        ChangeState(GameState.Finished);
        LoadScene("Menu");
    }

    public void LoadScene(string sceneToLoad)
    {
        Debug.Log($"Loading scene {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        OnGameQuit?.Invoke();
        Application.Quit();
    }
    
}
