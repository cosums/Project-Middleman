using System;
using System.Collections.Generic;
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

    private ContactFilter2D _playerFilter;
    private List<Collider2D> _overlaps = new();
    private CinemachineImpulseSource impulseSource;

    public void OnEnable()
    {
        PlayerController.OnPlayerAttack += HandlePlayerAttack;
        impulseSource = GetComponent<CinemachineImpulseSource>();

        _playerFilter = new ContactFilter2D();
        _playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        _playerFilter.useTriggers = true;
    }

    public void OnDisable()
    {
        PlayerController.OnPlayerAttack -= HandlePlayerAttack;
    }

    private bool IsPlayerOverlapping()
    {
        _overlaps.Clear();
        Collider.Overlap(_playerFilter, _overlaps);
        return _overlaps.Count > 0;
    }

    public void HandlePlayerAttack()
    {
        if (!IsPlayerOverlapping()) return;
        
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
