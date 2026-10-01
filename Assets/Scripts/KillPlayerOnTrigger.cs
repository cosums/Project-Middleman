using UnityEngine;

public class KillPlayerOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.CurrentState == GameState.Playing) 
            {
                PlayerDeathManager.Instance.KillPlayer();
            } else
            {
                PlayerDeathManager.Instance.ReplayKillPlayer();
            }
        }
    }
}
