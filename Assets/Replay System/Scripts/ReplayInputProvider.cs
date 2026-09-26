using UnityEngine;

public class ReplayInputProvider : IInputProvider
{
    private InputFrame[] _frames;
    private int _cursor;

    public bool IsFinished => _cursor >= _frames.Length;

    public ReplayInputProvider(InputFrame[] frames)
    {
        _frames = frames;
    }

    public InputFrame GetInput(int tick)
    {
        if (IsFinished) return default;

        var frame = _frames[_cursor];
        _cursor++;
        return frame;
    }
}
