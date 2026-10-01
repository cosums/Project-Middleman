using UnityEngine;

public class Train : MonoBehaviour
{
    private bool isRunning = false;
    public float maxSpeed;
    public float acceleration;
    private Rigidbody2D rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartTrain()
    {
        isRunning = true;
    }

    public void FixedUpdate()
    {
        if (rb.linearVelocity.x < maxSpeed && isRunning)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX + acceleration, rb.linearVelocityY);
        }
    }
}
