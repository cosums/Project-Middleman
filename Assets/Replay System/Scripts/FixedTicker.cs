using System;
using UnityEngine;
using UnityEngine.Profiling;

public static class FixedTickerUtility
{
    public static InputRecorder Recorder;
}

public class FixedTicker : MonoBehaviour
{
    public PlayerController Player;
    public LiveInputProvider LiveProvider;

    public int CurrentTick;

    private IInputProvider _inputProvider;
    public static event Action OnFinishedReplay;

    public bool EditorTest = false;

    void Awake()
    {
        OnFinishedReplay += ReturnToLive;
        GameManager.OnNewState += HandleNewState;

        Debug.Log("Goofy guber");
        
        if (EditorTest)
        {
            FixedTickerUtility.Recorder = new InputRecorder();
            Begin(LiveProvider);
            return;
        }

        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            FixedTickerUtility.Recorder = new InputRecorder();
            Begin(LiveProvider);
        } 
        else if (GameManager.Instance.CurrentState == GameState.Replaying)
        {
            InputFrame[] recordedFrames = FixedTickerUtility.Recorder.ToFrameArray();
            ReplayInputProvider replayProvider = new ReplayInputProvider(recordedFrames);
            Begin(replayProvider);
        }
    }

    public void Begin(IInputProvider inputProvder)
    {
        _inputProvider = inputProvder;
        CurrentTick = 0;
    }

    public void ReturnToLive()
    {
        FixedTickerUtility.Recorder = new InputRecorder();
        LiveProvider.ResetLatches();
        Player.CaptureStart();
        Begin(LiveProvider);
    }

    private void HandleNewState(GameState state)
    {
        if (state != GameState.Playing && FixedTickerUtility.Recorder != null)
        {
            FixedTickerUtility.Recorder.FinishRecording();
        }
    }

    private void FixedUpdate()
    {
        if (_inputProvider == null) return;

        if (_inputProvider.IsFinished)
        {
            _inputProvider = null;
            OnFinishedReplay?.Invoke();
            return;
        }

        var frame = _inputProvider.GetInput(CurrentTick);
        Player.SimulateTick(frame);
        FixedTickerUtility.Recorder.RecordFrame(frame);

        CurrentTick++;
    }
}
