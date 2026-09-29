using UnityEngine;

public class InsideFilmFrame : MonoBehaviour
{
    public AudioClip playerEntered;

    public float insideFrameMult = 0.01f;
    private bool playerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerInside = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        playerInside = false;
    }

    void Update()
    {
        if (playerInside)
        {
            PointSystem.Instance.AddToMultiplier(insideFrameMult, "In Frame!");
        }
    }
}
