using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<Transform> Waypoints;
    public float MoveSpeed;
    public Collider2D Collider;

    private int _waypointIndex;
    private int _dir = 1;

    void Start()
    {
        if (Waypoints.Count > 0)
        {
            transform.position = Waypoints[0].position;
        }
    }

    void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (_waypointIndex >= Waypoints.Count)
        {
            _waypointIndex--;
            _dir = -1;
        } 
        else if (_waypointIndex < 0)
        {
            _waypointIndex++;
            _dir = 1;
        }
        
        float step = MoveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, Waypoints[_waypointIndex].position, step);

        if (transform.position == Waypoints[_waypointIndex].position)
        {
            _waypointIndex += _dir;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        
        if (Waypoints == null || Waypoints.Count < 0) return;
        
        Vector3 lastPos = Waypoints[0].position;

        foreach (var w in Waypoints)
        {
            if (w == null) return;
            Gizmos.DrawLine(lastPos, w.position);
            lastPos = w.position;
        }
    }
}
