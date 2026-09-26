using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { MainMenu, Playing, Paused }
    public GameState CurrentState { get; private set; }
    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Debug.LogWarning(">1 Game Manager in scene!");
            Destroy(gameObject);
        }
    }

    
}
