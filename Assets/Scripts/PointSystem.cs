using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PointSystem : MonoBehaviour
{
    public static int points = 0;
    public static float multiplier = 1;

    public static Action OnMultiplerChanged;
    public static Action OnPointsChanged;

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

    public void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            AddScore(10);
        }
    }


}
