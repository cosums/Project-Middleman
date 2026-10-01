using UnityEngine;

public class CameraToggler : MonoBehaviour
{
    public void Start()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            gameObject.SetActive(false);
        } 
    }
}
