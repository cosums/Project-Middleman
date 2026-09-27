using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public Transform Player;
    private Vector3 _startOffset;

    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.1f;

    void Start()
    {
        _startOffset = transform.position - Player.position;
    }

    void LateUpdate()
    {
        Vector3 newPos = Player.position + _startOffset;
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }
}
