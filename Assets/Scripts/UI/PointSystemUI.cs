using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class PointSystemUI : MonoBehaviour
{
    [Header("Total Points UI")]
    [SerializeField] TextMeshProUGUI totalPointsTMP;
    [SerializeField] Animator totalPointsAnimator;

    [Header("Combo UI")]
    [SerializeField] GameObject comboParentPanel;
    [SerializeField] GameObject comboScorePanel;
    [SerializeField] TextMeshProUGUI comboTMP;
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

        if (GameManager.Instance.CurrentState == GameState.Replaying)
        {
            gameObject.SetActive(false);
        }
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
        totalPointsTMP.text = PointSystem.Points.ToString();
        totalPointsAnimator.SetTrigger("change");
    }

    private void UpdateComboPointsUI(string source, int points, float multiplier)
    {
        comboPointsTMP.text = PointSystem.Instance.ComboPoints.ToString();
        comboTMP.color = MultiplierToColor(multiplier);
        comboPointsTMP.color = MultiplierToColor(multiplier);
        comboPointsAnimator.SetTrigger("change");
    }

    private void UpdateComboPanelUI(string source, int points, float multiplier)
    {
        // No more combos
        if (PointSystem.Instance.ComboEntries.Count == 0)
        {
            Debug.Log("Empty Combo Panel");
            

            foreach (Transform child in comboEntriesPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            comboParentPanel.SetActive(false);

            return;
        }

        
        
        TextMeshProUGUI entry = Instantiate(comboEntryPrefab, comboEntriesPanel.transform).comboText;
        entry.text = "+" + points.ToString() + " " + source;
        entry.gameObject.SetActive(true);
        entry.color = MultiplierToColor(multiplier);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(comboEntriesPanel.GetComponent<RectTransform>());

        comboParentPanel.SetActive(true);
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
            return Color.aquamarine;
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