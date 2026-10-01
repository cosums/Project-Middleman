using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject speakerTextObject;
    [SerializeField] private GameObject contentTextObject;

    private TextMeshProUGUI speakerText;
    private TextMeshProUGUI contentText;

    public static DialogueDisplay Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        speakerText = speakerTextObject.GetComponent<TextMeshProUGUI>();
        contentText = contentTextObject.GetComponent<TextMeshProUGUI>();

        HideDialogue();
    }

    public void SetDialogueLine(DialogueLine line)
    {
        // Make sure the panel is active so Unity can calculate the layout.
        parent.SetActive(true);

        // Set the new text.
        speakerText.text = line.speaker + ":";
        contentText.text = line.contents;

        // Make sure TMP updates its preferred sizes immediately.
        speakerText.ForceMeshUpdate();
        contentText.ForceMeshUpdate();

        // Force the UI hierarchy to recalculate.
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            parent.GetComponent<RectTransform>()
        );
    }

    public void HideDialogue()
    {
        speakerText.enabled = false;
        contentText.enabled = false;
        parent.SetActive(false);
    }

    public void ShowDialogue()
    {
        parent.SetActive(true);

        speakerText.enabled = true;
        contentText.enabled = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            GetComponent<RectTransform>()
        );
    }
}