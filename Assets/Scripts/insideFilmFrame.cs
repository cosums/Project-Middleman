using UnityEngine;

public class insideFilmFrame : MonoBehaviour
{
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.layer == 7)
        {
            playerInside = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
