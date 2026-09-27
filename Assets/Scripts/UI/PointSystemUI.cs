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
    [SerializeField] TextMeshProUGUI comboEntryPrefab;

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

    private void UpdateComboPointsUI(string source)
    {
        comboPointsTMP.text = PointSystem.Instance.ComboPoints.ToString();
        comboPointsAnimator.SetTrigger("change");
    }

    private void UpdateComboPanelUI(string source)
    {
        foreach (Transform child in comboEntriesPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // No more combos
        if (PointSystem.Instance.ComboEntries.Count == 0)
        {
            Debug.Log("Empty");
            comboScorePanel.SetActive(false);
            return;
        }

        comboScorePanel.SetActive(true);

        foreach ((string s, int points) in PointSystem.Instance.ComboEntries)
        {
            TextMeshProUGUI entry = Instantiate(comboEntryPrefab, comboEntriesPanel.transform);
            entry.text = "+" + points.ToString() + s;
        }
    }

    #endregion

    #region Multiplier
    private void UpdateMultiplierUI(string source)
    {
        var multiplier = PointSystem.Instance.Multiplier;
        multiplierTMP.text = multiplier.ToString();
        UpdateMultiplierTextColor();
       
        multiplierAnimator.SetTrigger("change");
    }

    private void UpdateMultiplierTextColor()
    {
        if (PointSystem.Instance.Multiplier < 1.5)
        {
            multiplierTMP.color = Color.white;
        }
        else if (PointSystem.Instance.Multiplier < 3)
        {
            multiplierTMP.color = Color.orange;
        }
        else if (PointSystem.Instance.Multiplier < 5)
        {
            multiplierTMP.color = Color.red;
        }
        else
        {
            multiplierTMP.color = Color.purple;
        }
    }
    #endregion
}