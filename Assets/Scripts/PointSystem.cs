using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    private int points = 0;
    private int comboPoints = 0;
    private float multiplier = 1;

    [SerializeField] float comboDuration = 3; // if time since last point source is greater, bank the combo
    private float comboTimer = 0;
    private Coroutine comboRoutine;
    private List<(string pointSource, int points)> comboEntries
        = new List<(string pointSource, int points)>();

    public Action<string> OnMultiplierChanged;
    public Action<string> OnComboPointsChanged;
    public Action<string> OnPointsChanged;

    public int Points => points;
    public int ComboPoints => comboPoints;
    public float Multiplier => multiplier;
    public IReadOnlyList<(string pointSource, int points)> ComboEntries => comboEntries;

    public static PointSystem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region Multiplier
    public void AddToMultiplier(float m, string source)
    {
        multiplier += m;
        OnMultiplierChanged?.Invoke(source);
    }

    public void SetMultiplier(float m, string source)
    {
        multiplier = m;
        OnMultiplierChanged?.Invoke(source);
    }
    #endregion

    /// <summary>
    /// Add to combo score
    /// </summary>
    public void AddScore(int baseScore, string source)
    {
        int pointsToAdd = (int)(baseScore * multiplier);
        comboPoints += pointsToAdd;
        comboEntries.Add((source, pointsToAdd));
        OnComboPointsChanged?.Invoke(source);

        // reset the countdown
        comboTimer = comboDuration;

        // start the timer if it isn't already running
        if (comboRoutine == null)
        {
            comboRoutine = StartCoroutine(ComboTimer());
        }
    }

    private IEnumerator ComboTimer()
    {
        while (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("Combo time ended");
        points += comboPoints;
        OnPointsChanged?.Invoke("combo");

        // Clear combo state
        comboEntries.Clear();
        comboPoints = 0;
        OnComboPointsChanged?.Invoke("combo reset");
        comboRoutine = null;
    }

    public void AddToTotalScore(int baseScore, string source)
    {
        points += (int)(baseScore * multiplier);
        OnPointsChanged?.Invoke(source);
    }

    // For testing
    public void AddRandomPointSource()
    {
        AddScore(UnityEngine.Random.Range(10, 500), "bleh ");
    }
}