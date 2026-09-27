using TMPro;
using UnityEngine;

public class PointSystemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointsTMP;
    [SerializeField] Animator animator;

    private void Awake()
    {
        PointSystem.OnPointsChanged += UpdatePoints;
    }

    private void UpdatePoints()
    {
        pointsTMP.text = PointSystem.points.ToString();
        animator.SetTrigger("change");
    }
}
