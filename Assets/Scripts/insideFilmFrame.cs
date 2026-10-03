using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InsideFilmFrame : MonoBehaviour
{
    public Collider2D Collider;
    public AudioClip playerEntered;
    public static float KillCountDown = 4f;
    private bool _hasLost = false;
    private float _timeOutside = 0;

    [SerializeField] CountdownUI countdownUI;

    public Color[] CountDownColors = new Color[4];

    
    private ContactFilter2D _playerFilter;
    private List<Collider2D> _overlaps = new();

    private float _totalTime => KillCountDown + 1;

    public static InsideFilmFrame Instance { get; private set; }

    public bool inside;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (GameManager.Instance.CurrentState != GameState.Playing) {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        _timeOutside = 0;
        Collider = GetComponent<Collider2D>();

        _playerFilter = new ContactFilter2D();
        _playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        _playerFilter.useTriggers = true;

        countdownUI = Object.FindAnyObjectByType<CountdownUI>(FindObjectsInactive.Include);

        if (GameManager.Instance.CurrentState == GameState.Playing) {
            //UpdateCountdownUI();
        } else
        {
            countdownUI.gameObject.SetActive(false);
        }
    } 

    private bool IsPlayerOverlapping()
    {
        _overlaps.Clear();
        Collider.Overlap(_playerFilter, _overlaps);
        return _overlaps.Count > 0;
    }

    void Update()
    {
        if (_hasLost || GameManager.Instance.CurrentState != GameState.Playing) return;

        if (IsPlayerOverlapping())
        {
            inside = true;
            _timeOutside = 0f;
        }
        else
        {
            inside = false;
            _timeOutside += Time.deltaTime;

            if (_timeOutside >= _totalTime)
            {
                _timeOutside = _totalTime;
                _hasLost = true;
                AudioBus.Instance.FadeMusicOut(2.0f);
                GameManager.Instance.LoseGame(ResultCondition.OutOfFrame);
            }
        }

        if (GameManager.Instance.CurrentState == GameState.Playing) {
            UpdateCountdownUI();
        } else
        {
            countdownUI.gameObject.SetActive(false);
        }
    }

    private void UpdateCountdownUI()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;
        
        // Skip the first second so the UI first appears at KillCountDown (3)
        float graceTime = 1f;
        bool showCountdown = _timeOutside >= graceTime;
        countdownUI.gameObject.SetActive(showCountdown);
        if (!showCountdown) return;

        int displayNumber = Mathf.Max(0, Mathf.FloorToInt(_totalTime - _timeOutside));
        countdownUI.UpdateCountdownText(displayNumber.ToString());

        if (CountDownColors.Length > 0)
        {
            // Offset by the grace time so the first color maps to the first visible number
            int colorIndex = Mathf.Clamp(Mathf.FloorToInt(_timeOutside - graceTime), 0, CountDownColors.Length - 1);
            countdownUI.UpdateCountdownColor(CountDownColors[colorIndex]);
        }
    }
}
