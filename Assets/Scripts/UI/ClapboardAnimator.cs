using UnityEngine;
using UnityEngine.SceneManagement;

public class ClapboardAnimator : MonoBehaviour
{
    public static ClapboardAnimator Instance;

    public Animator ClapAnimator;

    public float TimeToClap;
    public float TimeAfterClap;
    public float FlyOutTime;

    public AudioClip ClapboardSFX;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        
        DontDestroyOnLoad(gameObject);
    }

    public async Awaitable FadeToScene(string scene)
    {
        ClapAnimator.SetTrigger("Start Load");
        
        await Awaitable.WaitForSecondsAsync(TimeToClap);

        AudioBus.Instance.PlaySFX(ClapboardSFX);
        
        await Awaitable.WaitForSecondsAsync(TimeAfterClap);
        
        await SceneManager.LoadSceneAsync(scene);

        ClapAnimator.SetTrigger("Finish Load");

        await Awaitable.WaitForSecondsAsync(FlyOutTime);

        GameManager.Instance.FinishLoadingScene();
    }
}
