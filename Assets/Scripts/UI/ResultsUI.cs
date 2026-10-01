using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] RectTransform parentPanel;

    [SerializeField] List<GameObject> resultsEntries = new List<GameObject>();

    [SerializeField] TextMeshProUGUI resultTMP;
    [SerializeField] TextMeshProUGUI overallScoreTMP;
    [SerializeField] TextMeshProUGUI highScoreTMP;

    [SerializeField] Color winColor;
    [SerializeField] Color loseColor;

    [SerializeField] AudioClip winSFX;
    [SerializeField] AudioClip loseSFX;
    [SerializeField] AudioClip whipCrackSFX;

    private void Start()
    {
        overallScoreTMP.text = "Overall Score: " + PointSystem.LastScore.ToString();
        highScoreTMP.text = "High Score: " + PointSystem.HighScore.ToString();
        UpdateResultText(GameManager.Instance.LastResult);
        if (GameManager.Instance.LastResult == ResultCondition.Win)
        {
            AudioBus.Instance.PlaySFX(winSFX);
        }
        else
        {
            AudioBus.Instance.PlaySFX(loseSFX);
        }


        StartCoroutine(AnimationSequence());
    }

    IEnumerator AnimationSequence()
    {
        ToggleEntries(false);

        yield return new WaitForSeconds(1.5f);

        foreach (var entry in resultsEntries)
        {
            entry.SetActive(true);

            AudioBus.Instance.PlaySFX(whipCrackSFX);

            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private void ToggleEntries(bool enable)
    {
        foreach (var entry in resultsEntries)
        {
            entry.SetActive(enable);
        }
    }

    private void UpdateResultText(ResultCondition result)
    {
        switch (result)
        {
            case ResultCondition.Win:
                resultTMP.text = "BLOCKBUSTER";
                resultTMP.color = winColor;
                break;
            case ResultCondition.Death:
                resultTMP.text = "DECEASED";
                resultTMP.color = loseColor;
                break;
            case ResultCondition.OutOfFrame:
                resultTMP.text = "OUT OF FRAME";
                resultTMP.color = loseColor;
                break;
        }
            
                
    }
}
