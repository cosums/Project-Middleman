using UnityEngine;

public class BirdUp : MonoBehaviour
{
    public float speed = 3f;
    public float amplitude = 0.5f;

    private SpriteRenderer _renderer;

    public void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (speed > 0)
        {
            _renderer.flipX = true;
        }
    }
    public void Update()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
    }
}
