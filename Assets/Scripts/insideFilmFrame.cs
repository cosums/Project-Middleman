using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InsideFilmFrame : MonoBehaviour
{
    public Collider2D Collider;
    public AudioClip playerEntered;
    public float KillCountDown = 3f;
    public float insideFrameMult = 0.01f;
    private bool _hasLost = false;
    private float _timeOutside = 0;

    public TMP_Text CountDownText;

    public Color[] CountDownColors = new Color[4];

    
    private ContactFilter2D _playerFilter;
    private List<Collider2D> _overlaps = new();

    private float _totalTime => KillCountDown + 1;

    void Start()
    {
        _timeOutside = 0;
        Collider = GetComponent<Collider2D>();

        _playerFilter = new ContactFilter2D();
        _playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        _playerFilter.useTriggers = true;
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
            PointSystem.Instance.AddToMultiplier(insideFrameMult, "In Frame!");
            _timeOutside = 0;
        }
        else
        {
            _timeOutside += Time.deltaTime;

            if (_timeOutside >= _totalTime)
            {
                _timeOutside = _totalTime;
                _hasLost = true;
                GameManager.Instance.LoseGame(DeathCondition.OutOfFrame);
            }
        }

        if (GameManager.Instance.CurrentState == GameState.Playing) {
            UpdateCountdownUI();
        } else
        {
            CountDownText.gameObject.SetActive(false);
        }
    }

    private void UpdateCountdownUI()
    {
        bool showCountdown = _timeOutside > 0f;
        CountDownText.gameObject.SetActive(showCountdown);
        if (!showCountdown) return;

        int displayNumber = Mathf.Max(0, Mathf.FloorToInt(_totalTime - _timeOutside));
        CountDownText.text = displayNumber.ToString();

        if (CountDownColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(Mathf.FloorToInt(_timeOutside), 0, CountDownColors.Length - 1);
            CountDownText.color = CountDownColors[colorIndex];
        }
    }
}
