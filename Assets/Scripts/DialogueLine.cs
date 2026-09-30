using System;
using UnityEngine;

// Plain data class (not a MonoBehaviour), so `new DialogueLine(...)` is valid
// and nothing about a line changes while it plays.
[Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea] public string contents;
    public AudioClip clip;
    public float minDuration;   // only used when there is no clip
    public bool hasEvent;

    public DialogueLine(string speaker, string contents, AudioClip clip)
    {
        this.speaker = speaker;
        this.contents = contents;
        this.clip = clip;
        this.minDuration = 0f;
        this.hasEvent = false;
    }

    public DialogueLine(string speaker, string contents, float minDuration)
    {
        this.speaker = speaker;
        this.contents = contents;
        this.clip = null;
        this.minDuration = minDuration;
        this.hasEvent = false;
    }

    public DialogueLine(string speaker, string contents, AudioClip clip, bool hasEvent)
    {
        this.speaker = speaker;
        this.contents = contents;
        this.clip = clip;
        this.minDuration = 0f;
        this.hasEvent = hasEvent;
    }

    public DialogueLine(string speaker, string contents, float minDuration, bool hasEvent)
    {
        this.speaker = speaker;
        this.contents = contents;
        this.clip = null;
        this.minDuration = minDuration;
        this.hasEvent = hasEvent;
    }

    // How long the line stays active: the clip length if there is a clip, otherwise minDuration.
    public float Duration => clip != null ? clip.length : minDuration;
}