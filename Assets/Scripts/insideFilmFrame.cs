using UnityEngine;

public class InsideFilmFrame : MonoBehaviour
{
    public AudioClip playerEntered;
    public float KillCountDown = 3f;
    public float insideFrameMult = 0.01f;
    private bool playerInside = false;
    private float _timeOutside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInside = true;
            _timeOutside = 0;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (playerInside)
        {
            PointSystem.Instance.AddToMultiplier(insideFrameMult, "In Frame!");
        }

        if (_timeOutside < KillCountDown)
        {
            _timeOutside += Time.deltaTime;
            if (_timeOutside >= KillCountDown)
            {
                GameManager.Instance.LoseGame();
            }
        } 
    }
}
