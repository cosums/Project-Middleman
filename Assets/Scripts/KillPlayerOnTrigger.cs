using UnityEngine;

public class KillPlayerOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerDeathManager.Instance.KillPlayer();
        }
    }
}
