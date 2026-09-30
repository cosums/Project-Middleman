using UnityEngine;

/// <summary>
/// Helper script used by animations to destroy or disable itself
/// </summary>
public class ObjectManager : MonoBehaviour
{
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void EnableObject()
    {
        gameObject.SetActive(true);
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
