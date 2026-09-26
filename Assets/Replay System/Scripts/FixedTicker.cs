using System;
using UnityEngine;

public class FixedTicker : MonoBehaviour
{
    public PlayerController Player;
    public LiveInputProvider LiveProvider;

    public int CurrentTick;

    private IInputProvider _inputProvider;
    public static event Action OnFinished;

    public bool EditorTest = false;

    void Awake()
    {
        if (EditorTest)
        {
            Begin(LiveProvider);
        }
    }

    public void Begin(IInputProvider inputProvder)
    {
        _inputProvider = inputProvder;
        CurrentTick = 0;
    }

    private void FixedUpdate()
    {
        if (_inputProvider == null) return;

        if (_inputProvider.IsFinished)
        {
            _inputProvider = null;
            OnFinished?.Invoke();
            return;
        }

        var frame = _inputProvider.GetInput(CurrentTick);
        Player.SimulateTick(frame);

        CurrentTick++;
    }
}
