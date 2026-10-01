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
        ClapAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public async Awaitable FadeToScene(string scene)
    {
        Time.timeScale = 0;
        
        ClapAnimator.SetTrigger("Start Load");
        
        await WaitUnscaled(TimeToClap);
        
        await WaitUnscaled(TimeAfterClap);
        
        await SceneManager.LoadSceneAsync(scene);

        ClapAnimator.SetTrigger("Finish Load");

        Time.timeScale = 1;

        await WaitUnscaled(FlyOutTime);

        GameManager.Instance.FinishLoadingScene();
    }

    private static async Awaitable WaitUnscaled(float seconds)
    {
        float end = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < end)
            await Awaitable.NextFrameAsync();
    }

    public void PlayClap()
    {
        AudioBus.Instance.PlaySFX(ClapboardSFX);
    }
}
