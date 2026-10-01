using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneUI : MonoBehaviour
{
    [SerializeField] List<RectTransform> CutsceneImages = new List<RectTransform>();
    [SerializeField] float flipDuration = 0.5f;
    [SerializeField] AudioClip pageFlipSFX;

    [SerializeField] GameObject skipCutsceneButton;

    private Canvas canvas;
    private RectTransform canvasRect;
    private int current = 0;
    private bool isFlipping = false;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void Start()
    {
        Vector2 size = canvasRect.rect.size;

        for (int i = 0; i < CutsceneImages.Count; i++)
        {
            RectTransform img = CutsceneImages[i];

            // Center-anchored so anchoredPosition is relative to the canvas center
            img.anchorMin = img.anchorMax = img.pivot = new Vector2(0.5f, 0.5f);
            img.sizeDelta = size;
            img.anchoredPosition = new Vector2(i * size.x, 0f);
        }
    }

    public void FlipPage()
    {
        if (isFlipping) return;
        if (current >= CutsceneImages.Count - 2)
        {
            skipCutsceneButton.SetActive(false);
            StartCoroutine(StartGameDelay());
        }
        else
        {
            StartCoroutine(SlideLeft());
        }

        AudioBus.Instance.PlaySFX(pageFlipSFX);
    }

    IEnumerator StartGameDelay()
    {
        yield return SlideLeft();

        GameManager.Instance.StartGame();
    }

    private IEnumerator SlideLeft()
    {
        isFlipping = true;

        float distance = canvasRect.rect.width;
        int count = CutsceneImages.Count;

        // Snapshot start positions so every image moves as one set
        Vector2[] starts = new Vector2[count];
        for (int i = 0; i < count; i++)
            starts[i] = CutsceneImages[i].anchoredPosition;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / flipDuration;
            float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));

            for (int i = 0; i < count; i++)
                CutsceneImages[i].anchoredPosition = starts[i] + Vector2.left * (distance * eased);

            yield return null;
        }

        current++;
        isFlipping = false;
    }
}