using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public Transform Player;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.x = Player.transform.position.x;
        transform.position = newPos;
    }
}
