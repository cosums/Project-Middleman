using UnityEngine;

public class FilmManager : MonoBehaviour
{
    public GameObject FilmCamera;
    
    public void Awake()
    {
        if (GameManager.Instance.CurrentState == GameState.Replaying)
        {
            FilmCamera.SetActive(true);
        }
    }

    public void FinishReplay()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.WinGame();
        } 
        else if (GameManager.Instance.CurrentState == GameState.Replaying)
        {
            GameManager.Instance.FinishReplay();
        }
    }

    public void QuickTimeEvent()
    {
        QuicktimeManager.Instance.AddQuickTimeEvent();
    }
}
