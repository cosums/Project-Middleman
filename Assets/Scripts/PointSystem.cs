using System;
using UnityEngine;
using UnityEngine.Events;
public class PointSystem : MonoBehaviour
{
    public static int points = 0;
    public static float multiplier = 1;
    [SerializeField] float comboTimer; // if time since last point source is greater, add up the points


    public static Action<string> OnMultiplierChanged;
    public static Action<string> OnPointsChanged;

    public void AddToMutliplier(float m, string source)
    {
        multiplier += m;
        OnMultiplierChanged.Invoke(source);
    }

    public void SetMultiplier(float m, string source)
    {
        multiplier = m;
        OnMultiplierChanged.Invoke(source);
    }

    public void AddScore(int baseScore, string source)
    {
        points += (int)(baseScore * multiplier);
        OnPointsChanged.Invoke(source);
    }

}
