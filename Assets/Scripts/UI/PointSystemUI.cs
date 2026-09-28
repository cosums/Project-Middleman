using TMPro;
using UnityEngine;

public class PointSystemUI : MonoBehaviour
{
    [Header("Total Points UI")]
    [SerializeField] TextMeshProUGUI totalPointsTMP;
    [SerializeField] Animator totalPointsAnimator;

    [Header("Combo UI")]
    [SerializeField] GameObject comboScorePanel;
    [SerializeField] TextMeshProUGUI comboPointsTMP;
    [SerializeField] Animator comboPointsAnimator;
    [SerializeField] GameObject comboEntriesPanel;
    [SerializeField] ComboEntryUI comboEntryPrefab;

    [Header("Multiplier UI")]
    [SerializeField] TextMeshProUGUI multiplierTMP;
    [SerializeField] Animator multiplierAnimator;

    private void Start()
    {
        PointSystem.Instance.OnPointsChanged += UpdateTotalPointsUI;
        PointSystem.Instance.OnComboPointsChanged += UpdateComboPointsUI;
        PointSystem.Instance.OnComboPointsChanged += UpdateComboPanelUI;
        PointSystem.Instance.OnMultiplierChanged += UpdateMultiplierUI;
    }

    private void OnDisable()
    {
        if (PointSystem.Instance == null) return;

        PointSystem.Instance.OnPointsChanged -= UpdateTotalPointsUI;
        PointSystem.Instance.OnComboPointsChanged -= UpdateComboPointsUI;
        PointSystem.Instance.OnComboPointsChanged -= UpdateComboPanelUI;
        PointSystem.Instance.OnMultiplierChanged -= UpdateMultiplierUI;
    }

    #region Points UI
    private void UpdateTotalPointsUI(string source)
    {
        totalPointsTMP.text = PointSystem.Instance.Points.ToString();
        totalPointsAnimator.SetTrigger("change");
    }

    private void UpdateComboPointsUI(string source, int points, float multiplier)
    {
        comboPointsTMP.text = PointSystem.Instance.ComboPoints.ToString();
        comboPointsAnimator.SetTrigger("change");
    }

    private void UpdateComboPanelUI(string source, int points, float multiplier)
    {
        // No more combos
        if (PointSystem.Instance.ComboEntries.Count == 0)
        {
            Debug.Log("Empty Combo Panel");
            return;
        }

        TextMeshProUGUI entry = Instantiate(comboEntryPrefab, comboEntriesPanel.transform).comboText;
        entry.text = "+" + points.ToString() + " " + source;
        entry.color = MultiplierToColor(multiplier);
    }

    #endregion

    #region Multiplier
    private void UpdateMultiplierUI(bool increasing)
    {
        var multiplier = PointSystem.Instance.Multiplier;
        multiplierTMP.text = "x" + multiplier.ToString("0.00##");
        UpdateMultiplierTextColor();

        if (increasing) multiplierAnimator.SetTrigger("change");
    }

    private void UpdateMultiplierTextColor()
    {
        multiplierTMP.color = MultiplierToColor(PointSystem.Instance.Multiplier);
    }

    private Color MultiplierToColor(float multiplier)
    {
        if (multiplier < 1.5)
        {
            return Color.white;
        }
        else if (multiplier < 3)
        {
            return Color.orange;
        }
        else if (multiplier < 5)
        {
            return Color.red;
        }
        else
        {
            return Color.purple;
        }
    }
    #endregion
}