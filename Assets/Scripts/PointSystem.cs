using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    [Header("Mulitplier")]
    [SerializeField] float minMultiplier = 0.5f;
    [SerializeField] float multiplierDuration = 3f; // delay before an individual source starts decaying
    [SerializeField] float multiplierDecayRate = 1f; // multiplier units lost per second while decaying
    [SerializeField] float airtimeMaxMult = 1f;
    [SerializeField] float slidingMaxMult = 1f;
    [SerializeField] float quicktimeMaxMult = 2.5f;
    [SerializeField] float insideFrameMaxMult = 1.5f;

    [Header("Multiplier Decay Delays")]
    [SerializeField] float airtimeDelay = 0.5f;
    [SerializeField] float slidingDelay = 1f;
    [SerializeField] float quicktimeDelay = 5f;
    [SerializeField] float insideFrameDelay = 0.25f;

    [Header("Combo")]
    [SerializeField] float comboDuration = 3;

    public static PointSystem Instance;
    public int Points => points;
    public int ComboPoints => comboPoints;
    public float Multiplier => multiplier;
    public IReadOnlyList<(string pointSource, int points)> ComboEntries => comboEntries;

    public Action<bool> OnMultiplierChanged;
    public Action<string, int, float> OnComboPointsChanged;
    public Action<string> OnPointsChanged;

    private int points = 0;
    private int comboPoints = 0;
    private float multiplier = 1;
    private float maxMultiplier = 5;
    private List<(string pointSource, int points)> comboEntries = new List<(string pointSource, int points)>();

    // Timers
    private float comboTimer = 0;
    private Coroutine comboRoutine;

    // --- Per-source multiplier state ---
    private class MultiplierSource
    {
        public float value;
        public float maxValue;
        public float timer;
        public float delay;
        public Coroutine timerRoutine;
        public Coroutine decayRoutine;
    }

    private MultiplierSource airtime = new MultiplierSource();
    private MultiplierSource sliding = new MultiplierSource();
    private MultiplierSource quicktime = new MultiplierSource();
    private MultiplierSource insideFrame = new MultiplierSource();

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

        airtime.maxValue = airtimeMaxMult;
        airtime.delay = airtimeDelay;

        sliding.maxValue = slidingMaxMult;
        sliding.delay = slidingDelay;

        quicktime.maxValue = quicktimeMaxMult;
        quicktime.delay = quicktimeDelay;

        insideFrame.maxValue = insideFrameMaxMult;
        insideFrame.delay = insideFrameDelay;

        maxMultiplier = minMultiplier + airtimeMaxMult + slidingMaxMult + quicktimeMaxMult + insideFrameMaxMult;
    }

    #region Multiplier
    private MultiplierSource GetSource(string source)
    {
        switch (source)
        {
            case "Air Time!": return airtime;
            case "Sliding!": return sliding;
            case "Pose on Queue!": return quicktime;
            case "In Frame!": return insideFrame;
            default:
                Debug.LogWarning($"Unknown multiplier source: {source}");
                return null;
        }
    }

    public void AddToMultiplier(float m, string source)
    {
        MultiplierSource src = GetSource(source);
        if (src == null) return;

        // Adding to this source cancels only ITS in-progress decay
        if (src.decayRoutine != null)
        {
            StopCoroutine(src.decayRoutine);
            src.decayRoutine = null;
        }

        src.value += m;
        if (src.value > src.maxValue) src.value = src.maxValue;
        if (src.value < 0) src.value = 0;

        RecalculateMultiplier();
        OnMultiplierChanged?.Invoke(true);

        // Reset (or start) this source's own delay-before-decay timer
        src.timer = src.delay;
        if (src.timerRoutine == null)
        {
            src.timerRoutine = StartCoroutine(SourceTimer(src));
        }
    }

    /// <summary>
    /// Per-source timer. Resets whenever that source is added to.
    /// When it hits 0, that source (only) starts decaying.
    /// </summary>
    private IEnumerator SourceTimer(MultiplierSource src)
    {
        while (src.timer > 0)
        {
            src.timer -= Time.deltaTime;
            yield return null;
        }

        src.timerRoutine = null;

        if (src.decayRoutine == null)
        {
            src.decayRoutine = StartCoroutine(SourceDecay(src));
        }
    }

    /// <summary>
    /// Decays a single source's value independently of all others.
    /// </summary>
    private IEnumerator SourceDecay(MultiplierSource src)
    {
        while (src.value > 0)
        {
            src.value -= multiplierDecayRate * Time.deltaTime;
            if (src.value < 0) src.value = 0;

            RecalculateMultiplier();
            OnMultiplierChanged?.Invoke(false);
            yield return null;
        }

        src.decayRoutine = null;
    }

    private void RecalculateMultiplier()
    {
        multiplier = minMultiplier + airtime.value + sliding.value + quicktime.value + insideFrame.value;

        if (multiplier > maxMultiplier) multiplier = maxMultiplier;
        else if (multiplier < minMultiplier) multiplier = minMultiplier;

        multiplier = MathF.Floor(multiplier * 100f) / 100f;
    }
    #endregion

    #region Combo Score
    public void AddToComboScore(int baseScore, string source)
    {
        int pointsToAdd = (int)(baseScore * multiplier);
        comboPoints += pointsToAdd;
        comboEntries.Add((source, pointsToAdd));
        OnComboPointsChanged?.Invoke(source, pointsToAdd, multiplier);

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

    public void AddRandomPointSource()
    {
        AddToComboScore(UnityEngine.Random.Range(10, 500), "bleh ");
    }
}