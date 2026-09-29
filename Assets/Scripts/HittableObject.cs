using System;
using Unity.Cinemachine;
using UnityEngine;

public class HittableObject : MonoBehaviour
{
    public Collider2D Collider;
    public GameObject DeathParticles;
    public AudioClip[] breakSounds;
    public int pointsGivenOnBreak;
    public string onHitScoreMessage;
    public float screenShakeAmount;

    public static Action ObjectHit;

    private bool _inPlayerAttackBox;
    private CinemachineImpulseSource impulseSource;

    public void OnEnable()
    {
        PlayerController.OnPlayerAttack += HandlePlayerAttack;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void OnDisable()
    {
        PlayerController.OnPlayerAttack -= HandlePlayerAttack;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _inPlayerAttackBox = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _inPlayerAttackBox = false;
        }
    }

    public void HandlePlayerAttack()
    {
        if (_inPlayerAttackBox)
        {
            if (DeathParticles != null) {
                GameObject particles = Instantiate(DeathParticles);
                particles.transform.position = transform.position;
                particles.GetComponent<ParticleSystem>().Play();
            }

            if (impulseSource != null)
            {
                impulseSource.GenerateImpulseWithForce(screenShakeAmount);
            }

            ObjectHit?.Invoke();

            PointSystem.Instance.AddToComboScore(pointsGivenOnBreak, onHitScoreMessage);

            AudioBus.Instance.PlayRandomSFX(breakSounds);

            Destroy(gameObject);
        }
    }
}
