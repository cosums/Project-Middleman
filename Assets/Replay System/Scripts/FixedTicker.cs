using System;
using UnityEngine;

public class FixedTicker : MonoBehaviour
{
    public PlayerController Player;
    public LiveInputProvider LiveProvider;
    private InputRecorder _recorder;

    public int CurrentTick;

    private IInputProvider _inputProvider;
    public static event Action OnFinishedReplay;

    public bool EditorTest = false;

    void Awake()
    {
        if (EditorTest)
        {
            Begin(LiveProvider);
        }

        _recorder = new InputRecorder();

        OnFinishedReplay += ReturnToLive;
    }

    public void Begin(IInputProvider inputProvder)
    {
        _inputProvider = inputProvder;
        CurrentTick = 0;
    }

    public void FinishAndReplay()
    {
        _recorder.FinishRecording();
        InputFrame[] recordedFrames = _recorder.ToFrameArray();
        ReplayInputProvider replayProvider = new ReplayInputProvider(recordedFrames);
        Begin(replayProvider);
    }

    public void ReturnToLive()
    {
        _recorder = new InputRecorder();
        LiveProvider.ResetLatches();
        Begin(LiveProvider);
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
        _recorder.RecordFrame(frame);

        CurrentTick++;
    }
}
