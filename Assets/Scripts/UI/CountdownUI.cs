using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdownTmp;

    public void UpdateCountdownText(string text)
    {
        countdownTmp.text = text;
    }

    public void UpdateCountdownColor(Color color)
    {
        countdownTmp.color = color;
    }




}
