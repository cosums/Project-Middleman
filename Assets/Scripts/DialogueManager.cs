using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class DialogueManager : MonoBehaviour
{
    private List<DialogueLine> dialogueQueue = new List<DialogueLine>();
    private DialogueLine head;

    private float _currentLineTime;
    private float _currentLineElapsed;

    public static DialogueManager Instance { get; private set; }

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

    public void EnqueueDialogue(DialogueLine dialogue)
    {
        dialogueQueue.Add(dialogue);
        if (dialogueQueue.Count == 1)
        {
            head = dialogue;
            head.Play();
            _currentLineTime = head.minDurationTimer;
        }
    }

    public void DequeueDialogue()
    {
        dialogueQueue.Remove(head);
        DialogueDisplay.Instance.HideDialogue();
        if (dialogueQueue.Count > 0)
        {
            head = dialogueQueue[0];
            head.Play();
            _currentLineTime = head.minDurationTimer;
        }
        else
        {
            head = null;
        }
    }

    public DialogueLine NewDialogueLine(string speaker, string content, AudioClip clip)
    {
        DialogueLine line = new DialogueLine();
        line.speaker = speaker;
        line.contents = content;
        line.clip = clip;

        return line;
    }

    public DialogueLine NewDialogueLine(string speaker, string content, float time)
    {
        DialogueLine line = new DialogueLine();
        line.speaker = speaker;
        line.contents = content;
        line.minDurationTimer = time;

        return line;
    }

    public void Update()
    {
        if (dialogueQueue.Count > 0 && head.clip == null)
        {
            if (_currentLineTime > 0)
            {
                _currentLineTime -= Time.deltaTime;
            }
            else
            {
                DequeueDialogue();
            }
        }
    }

    public DialogueLine GetHead()
    {
        return head;
    }
}

[Serializable]
public class DialogueLine : MonoBehaviour
{
    public string speaker;
    public string contents;
    public AudioClip clip;
    public float minDurationTimer;

    public bool playing = false;
        
    public void Init(string speaker, string contents, AudioClip audio)
    {
        this.speaker = speaker;
        this.contents = contents;
        this.clip = audio;
        this.minDurationTimer = -1f;
    }
    public void Init(string speaker, string contents, float minDuration)
    {
        this.speaker = speaker;
        this.contents = contents;
        this.clip = null;
        this.minDurationTimer = minDuration;
  
    }

    public void Play()
    {
        if (clip != null)
        {
            AudioBus.Instance.PlaySFX(clip);
            StartCoroutine(WaitForClipEnd());
        }
        DialogueDisplay.Instance.SetDialogueLine(this);
        DialogueDisplay.Instance.ShowDialogue();
    }

    private IEnumerator WaitForClipEnd()
    {
        yield return new WaitForSeconds(clip.length);

        DialogueManager.Instance.DequeueDialogue();
    }
}
