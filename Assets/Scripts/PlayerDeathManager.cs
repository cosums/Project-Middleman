using System.Collections;
using UnityEngine;

public class PlayerDeathManager : MonoBehaviour
{
    public static PlayerDeathManager Instance;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject playerSprites;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField] float screenShakeAmt;

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
        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence()
    {
        rb.bodyType = RigidbodyType2D.Static;

        Instantiate(deathParticles, transform);
        yield return new WaitForSeconds(0.15f);
        playerSprites.SetActive(false);

        yield return new WaitForSeconds(2);

        GameManager.Instance.LoseGame(DeathCondition.Death);
    }


}
