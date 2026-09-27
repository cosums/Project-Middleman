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
        newPos.y = Player.transform.position.y;
        transform.position = newPos;
    }
}
