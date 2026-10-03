using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private readonly List<DialogueLine> dialogueQueue = new List<DialogueLine>();
    private Coroutine playRoutine;
    private bool skipRequested;

    // Never stored separately, so it can't go stale:
    // the active line is always the front of the queue, or null when idle.
    public DialogueLine Head => dialogueQueue.Count > 0 ? dialogueQueue[0] : null;
    public DialogueLine GetHead() => Head;
    public bool IsPlaying => playRoutine != null;

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
        }
    }

    public DialogueLine NewDialogueLine(string speaker, string content, AudioClip clip)
        => new DialogueLine(speaker, content, clip);

    public DialogueLine NewDialogueLine(string speaker, string content, float time)
        => new DialogueLine(speaker, content, time);

    public void EnqueueDialogue(DialogueLine dialogue)
    {
        if (dialogue == null) return;

        dialogueQueue.Add(dialogue);
        if (playRoutine == null)
            playRoutine = StartCoroutine(PlayQueue());
    }

    // Ends the current line early; the next one (if any) starts right after.
    // Note: this doesn't stop the voice clip itself, that depends on your AudioBus.
    public void SkipCurrent()
    {
        if (IsPlaying) skipRequested = true;
    }

    // The only place lines are started and removed, so there is no way to double-dequeue.
    private IEnumerator PlayQueue()
    {
        while (dialogueQueue.Count > 0)
        {
            DialogueLine line = dialogueQueue[0];   // == Head for the whole time it plays

            if (DialogueDisplay.Instance != null)
            {
                DialogueDisplay.Instance.SetDialogueLine(line);
                DialogueDisplay.Instance.ShowDialogue();
            }
            if (line.clip != null)
                AudioBus.Instance.PlaySFX(line.clip, 1, false);

            float elapsed = 0f;
            while (elapsed < line.Duration && !skipRequested)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            skipRequested = false;

            dialogueQueue.RemoveAt(0);
            if (DialogueDisplay.Instance != null)
                DialogueDisplay.Instance.HideDialogue();
        }

        playRoutine = null;
    }

    public bool CurrentDialogueHasEvent()
    {
        if (Head != null)
        {
            return Head.hasEvent;
        }
        else return false;
    }
}