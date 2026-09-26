using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialBoot : MonoBehaviour
{
    public string MainScene;
    
    void Start()
    {
        SceneManager.LoadScene(MainScene);
    }
}
