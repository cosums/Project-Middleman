using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject pointAObj;
    public GameObject pointBObj;
    public float speed = 3f;

    private Vector3 pointA;
    private Vector3 pointB;
    private Rigidbody2D rb;
    private Vector3 targetPos;
    private Vector2 targetVector;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pointA = pointAObj.transform.position;
        pointB = pointBObj.transform.position;
        targetPos = pointA;
        Switch();
    }

    private void Switch()
    {
        if (targetPos == pointA)
        {
            targetPos = pointB;
            targetVector = (pointB - pointA).normalized * speed;
        }
        else
        {
            targetPos = pointA;
            targetVector = (pointA - pointB).normalized * speed;
        }

        rb.linearVelocity = targetVector;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(rb.position, targetPos) < 0.01f)
        {
            Switch();
        }
    }
}
