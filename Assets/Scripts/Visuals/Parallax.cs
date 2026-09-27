using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    [SerializeField] private float effectMultipler;
    private GameObject cam;
    Vector3 camStartPos;
    Vector3 objStartPos;

    void Awake()
    {
        cam = GameObject.Find("Main Camera");
        objStartPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        camStartPos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
    }
    void Update()
    {
        Vector2 dist = new Vector2((cam.transform.position.x - camStartPos.x),
            (cam.transform.position.y - camStartPos.y)) * effectMultipler;
        transform.position = new Vector3(objStartPos.x + dist.x, objStartPos.y + dist.y, transform.position.z);
    }
}
