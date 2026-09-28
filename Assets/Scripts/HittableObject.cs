using System;
using UnityEngine;

public class HittableObject : MonoBehaviour
{
    public Collider2D Collider;
    public GameObject DeathParticles;

    public static Action ObjectHit;

    private bool _inPlayerAttackBox;

    public void OnEnable()
    {
        PlayerController.OnPlayerAttack += HandlePlayerAttack;
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
        if (collision.tag == "Player")
        {
            _inPlayerAttackBox = false;
        }
    }

    public void HandlePlayerAttack()
    {
        if (_inPlayerAttackBox)
        {
            if (DeathParticles != null) {
                Instantiate(DeathParticles);
                DeathParticles.transform.position = transform.position;
                DeathParticles.GetComponent<ParticleSystem>().Play();
            }

            ObjectHit?.Invoke();

            Destroy(gameObject);
        }
    }
}
