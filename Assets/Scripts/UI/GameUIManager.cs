using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameObject PointUI, DialogueUI, ReplayUI;

    void Start()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            PointUI.SetActive(false);
            DialogueUI.SetActive(false);
            ReplayUI.SetActive(true);
        }
    }

}
