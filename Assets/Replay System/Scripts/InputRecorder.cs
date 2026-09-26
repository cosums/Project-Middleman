using System.Collections.Generic;
using UnityEngine;

public class InputRecorder
{
    private List<(InputFrame, int holdTicks)> _runs = new();
    private InputFrame _lastFrame;
    private bool _hasFirstFrame;
    private int _lastHoldStart;
    private int _tickCount;
    
    public void RecordFrame(in InputFrame frame)
    {
        if (!_hasFirstFrame)
        {
            _lastFrame = frame;
            _hasFirstFrame = true;
            _lastHoldStart = frame.Tick;
        } else if (!FramesEqual(frame, _lastFrame))
        {
            _runs.Add((_lastFrame, frame.Tick - _lastHoldStart));
            _lastFrame = frame;
            _lastHoldStart = frame.Tick;
        }

        _tickCount = frame.Tick + 1;
    }

    public void FinishRecording()
    {
        if (_hasFirstFrame) _runs.Add((_lastFrame, _tickCount - _lastHoldStart));
    }

    private static bool FramesEqual(in InputFrame a, in InputFrame b)
    {
        return  a.Move == b.Move && 
                a.JumpWasPressed == b.JumpWasPressed &&
                a.JumpIsHeld == b.JumpIsHeld &&
                a.JumpWasReleased == b.JumpWasReleased;
    }
}
