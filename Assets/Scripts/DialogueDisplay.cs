using TMPro;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    public GameObject speakerTextObject;
    public GameObject contentTextObject;

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
        }

        speakerText = speakerTextObject.GetComponent<TextMeshProUGUI>();
        contentText = contentTextObject.GetComponent<TextMeshProUGUI>();
        
        HideDialogue();
    }

    public void SetDialogueLine(DialogueLine line)
    {
        speakerText.text = line.speaker + ":";
        contentText.text = line.contents;
    }

    public void HideDialogue()
    {
        speakerText.enabled = false;
        contentText.enabled = false;
    }

    public void ShowDialogue()
    {
        speakerText.enabled = true;
        contentText.enabled = true;
    }
}
