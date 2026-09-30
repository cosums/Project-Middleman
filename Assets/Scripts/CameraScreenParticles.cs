using UnityEngine;

public class CameraScreenParticles : MonoBehaviour
{
    private bool quickTime;
    [SerializeField] public PlayerController playerController;
    [SerializeField] public ParticleSystem activeQuickTimeParticles;
    [SerializeField] public ParticleSystem burstQuickTimeParticles;

    public void Start()
    {
        activeQuickTimeParticles.Stop();
    }

    public void Update()
    {
        if (playerController.IsInQuicktime() && quickTime == false)
        {
            quickTime = true;
            ParticleSystem burst = Instantiate(burstQuickTimeParticles);
            burst.transform.position = activeQuickTimeParticles.transform.position;
            activeQuickTimeParticles.Play();
        }
        if (playerController.IsInQuicktime() && quickTime == true) {
            activeQuickTimeParticles.Stop();
            quickTime = false;
        }
    }
}
