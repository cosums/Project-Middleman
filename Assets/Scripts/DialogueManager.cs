using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    private List<DialogueLine> dialogueQueue = new List<DialogueLine>();
    private DialogueLine head;

    public void EnqueueDialogue(DialogueLine dialogue)
    {
        dialogueQueue.Add(dialogue);
        if (dialogueQueue.Count == 1)
        {
            head = dialogue;
        }
    }

    public void DequeueDialogue()
    {
        dialogueQueue.Remove(head);
        if (dialogueQueue.Count > 0)
        {
            head = dialogueQueue[0];
        }
        else
        {
            head = null;
        }
    }

    public DialogueLine GetHead()
    {
        return head;
    }
}

[Serializable]
public class DialogueLine
{
    public string speaker;
    public string contents;
    public AudioClip audio;
    public float minDuration;
        
    public DialogueLine(string speaker, string contents, AudioClip audio, float minDuration)
    {
        this.speaker = speaker;
        this.contents = contents;
        this.audio = audio;
        this.minDuration = minDuration;
    }

    public void Play()
    {

    }
}
