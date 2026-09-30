using TMPro;
using UnityEngine;

public class InsideFilmFrame : MonoBehaviour
{
    public AudioClip playerEntered;
    public float KillCountDown = 3f;
    public float insideFrameMult = 0.01f;
    private bool _hasLost = false;
    private bool playerInside = false;
    private float _timeOutside = 0;

    public TMP_Text CountDownText;

    public Color[] CountDownColors = new Color[4];

    private float _totalTime => KillCountDown + 1;

    void Start()
    {
        _timeOutside = 0;
    } 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInside = true;
            _timeOutside = 0;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (_hasLost) return;

        if (playerInside)
        {
            PointSystem.Instance.AddToMultiplier(insideFrameMult, "In Frame!");
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

        UpdateCountdownUI();
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
