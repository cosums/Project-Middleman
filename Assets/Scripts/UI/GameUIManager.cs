using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameObject PointUI, DialogueUI, FailUI;

    public void OnEnable()
    {
        GameManager.OnNewState += HandleNewState;
    }

    public void OnDisable()
    {
        GameManager.OnNewState -= HandleNewState;
    }

    void Start()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            gameObject.SetActive(false);
        }
    }

    public void HandleNewState(GameState state)
    {
        if (state == GameState.Failed)
        {
            FailUI.SetActive(true);
        }
    }
}
