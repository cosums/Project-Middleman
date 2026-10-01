using UnityEngine;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public string SceneToLoad;
    
    Button MenuButton;
    
    public void Awake()
    {
        MenuButton = GetComponent<Button>();
    }

    public void OnEnable()
    {
        MenuButton.onClick.AddListener(LoadScene);
    }

    public void OnDisable()
    {
        MenuButton.onClick.RemoveListener(LoadScene);
    }

    private void LoadScene()
    {
        GameManager.Instance.ClapLoadScene(SceneToLoad);
    }
}
