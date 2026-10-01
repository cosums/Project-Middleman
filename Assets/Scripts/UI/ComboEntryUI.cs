using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ComboEntryUI : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI comboText;

    [Header("Slide In")]
    [SerializeField] private RectTransform slideInObject;
    [SerializeField] private float slideInDuration = 0.4f;
    [SerializeField] private float slideOffScreenDistance = 300f;
    [SerializeField] private AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade Out")]
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private CanvasGroup _canvasGroup;
    private Vector2 _targetAnchoredPos;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (slideInObject == null)
        {
            Debug.LogWarning($"{nameof(ComboEntryUI)} on {name} has no {nameof(slideInObject)} assigned.", this);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(SlideIn());
        Invoke(nameof(PlayFadeOutAndDestroy), 2f);
    }

    private IEnumerator SlideIn()
    {
        if (slideInObject == null)
        {
            PlayFadeOutAndDestroy();
            yield break;
        }

        // Wait for Unity's layout system to position the entry.
        yield return null;

        _targetAnchoredPos = slideInObject.anchoredPosition;

        Vector2 startPos =
            _targetAnchoredPos + Vector2.left * slideOffScreenDistance;

        float elapsed = 0f;

        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;

            float t = slideInCurve.Evaluate(
                Mathf.Clamp01(elapsed / slideInDuration)
            );

            slideInObject.anchoredPosition =
                Vector2.LerpUnclamped(startPos, _targetAnchoredPos, t);

            yield return null;
        }

        slideInObject.anchoredPosition = _targetAnchoredPos;
    }

    public void PlayFadeOutAndDestroy()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0f;

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeOutDuration);
            _canvasGroup.alpha = startAlpha * fadeOutCurve.Evaluate(t);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }
}