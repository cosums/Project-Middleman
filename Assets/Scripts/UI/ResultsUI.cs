using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI overallScoreTMP;
    [SerializeField] TextMeshProUGUI highScoreTMP;

    private void Start()
    {
        overallScoreTMP.text = "Overall Score: " + PointSystem.LastScore.ToString();
        highScoreTMP.text = "High Score: " + PointSystem.HighScore.ToString();
    }

}
