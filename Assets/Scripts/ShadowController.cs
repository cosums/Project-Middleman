using UnityEngine;

public class ShadowController : MonoBehaviour
{
    public Transform Player;
    public float MaxDistance;
    public float YOffset;
    public LayerMask GroundMask;

    public bool ScaleWithHeight = true;
    public float MinScale = 0.4f;
    public float FadeDistance = 5f;

    private Vector3 _baseScale;
    
    private SpriteRenderer _renderer;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _baseScale = transform.localScale;
    }

    void LateUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            Player.position, Vector2.down, MaxDistance, GroundMask);

        if (hit.collider != null)
        {
            _renderer.enabled = true;
            transform.position = new Vector3(transform.position.x,
                                          hit.point.y + YOffset,
                                          transform.position.z);

            
            float t = Mathf.Clamp01(hit.distance / FadeDistance);
            transform.localScale = Vector3.Lerp(_baseScale, _baseScale * MinScale, t);

            Color c = _renderer.color;
            c.a = Mathf.Lerp(1f, 0.3f, t);
            _renderer.color = c;
            
        }
        else
        {
            _renderer.enabled = false;
        }
    }
}
