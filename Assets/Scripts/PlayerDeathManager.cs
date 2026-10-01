using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerDeathManager : MonoBehaviour
{
    public static PlayerDeathManager Instance;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject playerSprites;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField] float screenShakeAmt;

    public AudioClip deathAudio;
    public AudioClip loseAudio;

    private bool isDead = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void KillPlayer()
    {
        if (isDead) return;

        isDead = true;
        AudioBus.Instance.PlaySFX(deathAudio);
        AudioBus.Instance.FadeMusicOut(2.0f);

        StartCoroutine(DieSequence());

    }

    IEnumerator DieSequence()
    {
        rb.bodyType = RigidbodyType2D.Static;

        Instantiate(deathParticles, transform);

        GetComponent<CinemachineImpulseSource>().GenerateImpulse(screenShakeAmt);

        yield return new WaitForSeconds(0.15f);        
        
        playerSprites.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        AudioBus.Instance.PlaySFX(loseAudio, 1.0f, false);

        yield return new WaitForSeconds(2);

        GameManager.Instance.LoseGame(ResultCondition.Death);
    }

    public void ReplayKillPlayer()
    {
        if (isDead) return;

        isDead = true;

        AudioBus.Instance.PlaySFX(deathAudio);
        AudioBus.Instance.FadeMusicOut(2.0f);

        StartCoroutine(ReplayDeathSequence());
    }

    IEnumerator ReplayDeathSequence()
    {
        rb.bodyType = RigidbodyType2D.Static;

        Instantiate(deathParticles, transform);

        GetComponent<CinemachineImpulseSource>().GenerateImpulse(screenShakeAmt);

        yield return new WaitForSeconds(0.15f);        
        
        playerSprites.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        AudioBus.Instance.PlaySFX(loseAudio, 1.0f, false);
    }
}
