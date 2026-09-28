using UnityEngine;

public class insideFilmFrame : MonoBehaviour
{
    public float insideFrameMult = 0.01f;
    private bool playerInside = false;

    private void OnTriggerEnter2D(UnityEngine.Collider2D other)
    {
          print("entered");
          playerInside = true;
    }
    private void OnTriggerExit2D(UnityEngine.Collider2D other)
    {
          playerInside = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInside)
        {
            PointSystem.Instance.AddToMultiplier(insideFrameMult, "In Frame!");
        }
    }
}
