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

    public void HandleNewState(GameState state)
    {
        if (state == GameState.Failed)
        {
            FailUI.SetActive(true);
        } else
        {
            FailUI.SetActive(false);
        }
    }
}
