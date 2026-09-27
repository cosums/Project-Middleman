using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class PointSystem : MonoBehaviour
{
    public static PointSystem Instance;
    public int Points => points;
    public int ComboPoints => comboPoints;
    public float Multiplier => multiplier;
    public IReadOnlyList<(string pointSource, int points)> ComboEntries => comboEntries;

    public Action<string> OnMultiplierChanged;
    public Action<string> OnComboPointsChanged;
    public Action<string> OnPointsChanged;

    private int points = 0;
    private int comboPoints = 0;
    private float multiplier = 1;
    private List<(string pointSource, int points)> comboEntries = new List<(string pointSource, int points)>();

    [Header("Timers")]
    [SerializeField] float comboDuration = 3; // if time since last point source is greater, bank the combo
    private float comboTimer = 0;
    private Coroutine comboRoutine;

    [SerializeField] float multiplierDuration = 3; // if time since last multiplier change is greater, decrease multiplier
    private float multiplierTimer = 0;
    private Coroutine multiplierRoutine;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        OnMultiplierChanged?.Invoke("");
        OnComboPointsChanged?.Invoke("");
        OnPointsChanged?.Invoke("");
    }

    #region Multiplier
    public void AddToMultiplier(float m, string source)
    {
        multiplier += m;
        multiplier = MathF.Floor(multiplier * 100f) / 100f;
        OnMultiplierChanged?.Invoke(source);

        // start combo timer
        multiplierTimer = multiplierDuration;
        if (multiplierRoutine == null) multiplierRoutine = StartCoroutine(MultiplierTimer());
    }

    private IEnumerator MultiplierTimer()
    {
        while (multiplierTimer > 0)
        {
            multiplierTimer -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("Rest multiplier");
        multiplier = 0.5f;
        OnMultiplierChanged?.Invoke("Reset");
        multiplierRoutine = null;

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

        // start combo timer
        comboTimer = comboDuration;
        if (comboRoutine == null) comboRoutine = StartCoroutine(ComboTimer());
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