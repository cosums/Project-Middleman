using TMPro;
using UnityEngine;

public class PointSystemUI : MonoBehaviour
{
    [Header("Total Points UI")]
    [SerializeField] TextMeshProUGUI totalPointsTMP;
    [SerializeField] Animator totalPointsAnimator;

    [Header("Combo UI")]
    [SerializeField] GameObject comboPanel;
    [SerializeField] TextMeshProUGUI comboPointsTMP;
    [SerializeField] Animator comboPointsAnimator;
    [SerializeField] TextMeshProUGUI comboEntryPrefab;

    private void Start()
    {
        PointSystem.Instance.OnPointsChanged += UpdateTotalPointsUI;
        PointSystem.Instance.OnComboPointsChanged += UpdateComboPointsUI;
        PointSystem.Instance.OnComboPointsChanged += UpdateComboPanelUI;
    }

    private void OnDisable()
    {
        if (PointSystem.Instance == null) return;

        PointSystem.Instance.OnPointsChanged -= UpdateTotalPointsUI;
        PointSystem.Instance.OnComboPointsChanged -= UpdateComboPointsUI;
        PointSystem.Instance.OnComboPointsChanged -= UpdateComboPanelUI;
    }

    private void UpdateTotalPointsUI(string source)
    {
        totalPointsTMP.text = PointSystem.Instance.Points.ToString();
        totalPointsAnimator.SetTrigger("change");
    }

    private void UpdateComboPointsUI(string source)
    {
        comboPointsTMP.text = PointSystem.Instance.ComboPoints.ToString();
        comboPointsAnimator.SetTrigger("change");
    }

    private void UpdateComboPanelUI(string source)
    {
        foreach (Transform child in comboPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (PointSystem.Instance.ComboEntries.Count == 0)
        {
            Debug.Log("Empty");
            return;
        }


        foreach ((string s, int points) in PointSystem.Instance.ComboEntries)
        {
            TextMeshProUGUI entry = Instantiate(comboEntryPrefab, comboPanel.transform);
            entry.text = "+" + points.ToString() + s;
        }
    }
}