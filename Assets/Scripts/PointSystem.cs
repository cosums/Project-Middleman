using UnityEngine;
using UnityEngine.Events;

public class PointSystem : MonoBehaviour
{
    public static int points = 0;
    public static float multiplier = 1;

    public static UnityEvent OnMultiplerChanged;
    public static UnityEvent OnPointsChanged;

    public void AddToMuliplier(float m)
    {
        multiplier += m;
        OnMultiplerChanged.Invoke();
    }

    public void SetMuliplier(float m)
    {
        multiplier = m;
        OnMultiplerChanged.Invoke();
    }

    public void AddScore(int baseScore)
    {
        points += (int)(baseScore * multiplier);
        OnPointsChanged.Invoke();
    }




}
