using UnityEngine;

public class insideFilmFrame : MonoBehaviour
{
    public float insideFrameMult = 0.01f;
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.layer == 7)
        {
            playerInside = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.layer == 7)
        {
            playerInside = false;
        }
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
