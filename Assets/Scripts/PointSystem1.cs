using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem1 : MonoBehaviour
{
    [Header("Mulitplier")]
    [SerializeField] float minMultiplier = 0.5f;
    [SerializeField] float multiplierDuration = 3f; // if time since last multiplier change is greater, start decaying
    [SerializeField] float multiplierDecayRate = 1f; // multiplier units lost per second while decaying
    [SerializeField] float airtimeMaxMult = 1f;
    [SerializeField] float slidingMaxMult = 1f;
    [SerializeField] float quicktimeMaxMult = 1f;
    [SerializeField] float insideFrameMaxMult = 1.5f;

    [Header("Combo")]
    [SerializeField] float comboDuration = 3; // if time since last point source is greater, bank the combo

    public static PointSystem1 Instance;
    public int Points => points;
    public int ComboPoints => comboPoints;
    public float Multiplier => multiplier;
    public IReadOnlyList<(string pointSource, int points)> ComboEntries => comboEntries;

    public Action<bool> OnMultiplierChanged; // bool isIncreasing
    public Action<string, int, float> OnComboPointsChanged; // string source, int points, float currMultiplier
    public Action<string> OnPointsChanged;

    private int points = 0;
    private int comboPoints = 0;
    private float multiplier = 1;
    private float maxMultiplier = 5;
    private float airtimeMult = 0f;
    private float slidingMult = 0f;
    private float quicktimeMult = 0f;
    private float insideFrameMult = 0f;
    private List<(string pointSource, int points)> comboEntries = new List<(string pointSource, int points)>();
    

    // Timers
    private float comboTimer = 0;
    private Coroutine comboRoutine;
    private float multiplierTimer = 0;
    private Coroutine multiplierRoutine;
    private Coroutine multiplierDecayRoutine;
    private float airtimeMultTimer = 0f;
    private float slidingMultTimer = 0f;
    private float quicktimeMultTimer = 0f;
    private float insideFrameMultTimer = 0f;


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
        OnMultiplierChanged?.Invoke(false);
        OnComboPointsChanged?.Invoke("", 0, 0);
        OnPointsChanged?.Invoke("");
        maxMultiplier = minMultiplier + airtimeMaxMult + slidingMaxMult + quicktimeMaxMult + insideFrameMaxMult;
    }

    #region Multiplier
    public void AddToMultiplier(float m, string source)
    {
        // Adding to the multiplier cancels any in-progress decay
        if (multiplierDecayRoutine != null)
        {
            StopCoroutine(multiplierDecayRoutine);
            multiplierDecayRoutine = null;
        }

        if (source == "Air Time!")
        {
            airtimeMult += m;
            if (airtimeMult > airtimeMaxMult)
            {
                airtimeMult = airtimeMaxMult;
            }
        }
        else if (source == "Sliding!")
        {
            slidingMult += m;
            if (slidingMult > slidingMaxMult)
            {
                slidingMult = slidingMaxMult;
            }
        }
        else if (source == "Pose on Queue!")
        {
            quicktimeMult += m;
            if (quicktimeMult > quicktimeMaxMult)
            {
                quicktimeMult = quicktimeMaxMult;
            }
        }
        else if (source == "In Frame!")
        {
            insideFrameMult += m;
            if (insideFrameMult > insideFrameMaxMult)
            {
                insideFrameMult = insideFrameMaxMult;
            }
        }

        multiplier = minMultiplier + airtimeMult + quicktimeMult + slidingMult + insideFrameMult;
        if (multiplier > maxMultiplier)
        {
            multiplier = maxMultiplier;
        }
        else if (multiplier < minMultiplier)
        {
            multiplier = minMultiplier;
        }
        multiplier = MathF.Floor(multiplier * 100f) / 100f;
        OnMultiplierChanged?.Invoke(true);

        multiplierTimer = multiplierDuration;
        if (multiplierRoutine == null) multiplierRoutine = StartCoroutine(MultiplierTimer());
    }

    /// <summary>
    /// Timer resets after multiplier is updated
    /// When timer hits 0, start decaying multiplier value
    /// </summary>
    /// <returns></returns>
    private IEnumerator MultiplierTimer()
    {
        while (multiplierTimer > 0)
        {
            multiplierTimer -= Time.deltaTime;
            yield return null;
        }

        multiplierRoutine = null;

        if (multiplierDecayRoutine == null)
        {
            multiplierDecayRoutine = StartCoroutine(MultiplierDecay());
        }
    }

    /// <summary>
    /// Decreases the multiplier by a certain rate
    /// </summary>
    /// <returns></returns>
    private IEnumerator MultiplierDecay()
    {
        float activeMultipliers = 0;

        if (airtimeMult > 0)
        {
            activeMultipliers += airtimeMaxMult;
        }
        if (slidingMult > 0)
        {
            activeMultipliers += slidingMaxMult;
        }
        if (quicktimeMult > 0)
        {
            activeMultipliers += quicktimeMaxMult;
        }
        if (insideFrameMult > 0)
        {
            activeMultipliers += insideFrameMaxMult;
        }

        while (multiplier > minMultiplier)
        {
            
            if (airtimeMult > 0)
            {
                airtimeMult -= airtimeMaxMult / activeMultipliers * Time.deltaTime;
                if (airtimeMult < 0)
                {
                    airtimeMult = 0;
                    activeMultipliers -= airtimeMaxMult;
                }
            }
            if (slidingMult > 0)
            {
                slidingMult -= slidingMaxMult / activeMultipliers * Time.deltaTime;
                if (slidingMult < 0)
                {
                    slidingMult = 0;
                    activeMultipliers -= slidingMaxMult;
                }
            }
            if (quicktimeMult > 0)
            {
                quicktimeMult -= quicktimeMaxMult / activeMultipliers * Time.deltaTime;
                if (quicktimeMult < 0)
                {
                    quicktimeMult = 0;
                    activeMultipliers -= quicktimeMaxMult;
                }
            }
            if (insideFrameMult > 0)
            {
                insideFrameMult -= insideFrameMaxMult / activeMultipliers * Time.deltaTime;
                if (insideFrameMult < 0)
                {
                    insideFrameMult = 0;
                    activeMultipliers -= insideFrameMaxMult;
                }
            }

            multiplier = minMultiplier + airtimeMult + quicktimeMult + slidingMult + insideFrameMult;
            multiplier = MathF.Floor(multiplier * 100f) / 100f;
            if (multiplier < minMultiplier)
            {
                multiplier = minMultiplier;
            }

            OnMultiplierChanged?.Invoke(false);
            yield return null;
        }

        Debug.Log("Multiplier decayed to minimum");
        multiplierDecayRoutine = null;
    }

    #endregion

    #region Combo Score
    public void AddToComboScore(int baseScore, string source)
    {
        int pointsToAdd = (int)(baseScore * multiplier);
        comboPoints += pointsToAdd;
        comboEntries.Add((source, pointsToAdd));
        OnComboPointsChanged?.Invoke(source, pointsToAdd, multiplier);

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

        ClearComboState();
    }

    private void ClearComboState()
    {
        comboEntries.Clear();
        comboPoints = 0;
        OnComboPointsChanged?.Invoke("reset", 0, 0);
        comboRoutine = null;
    }
    #endregion

    public void AddToTotalScore(int baseScore, string source)
    {
        points += (int)(baseScore * multiplier);
        OnPointsChanged?.Invoke(source);
    }

    // For testing
    public void AddRandomPointSource()
    {
        AddToComboScore(UnityEngine.Random.Range(10, 500), "bleh ");
    }
}