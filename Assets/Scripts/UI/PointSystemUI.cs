using TMPro;
using UnityEngine;

public class PointSystemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointsTMP;
    
    private void Awake()
    {
        PointSystem.OnPointsChanged.AddListener(UpdatePoints);
    }

    private void UpdatePoints()
    {

    }
}
