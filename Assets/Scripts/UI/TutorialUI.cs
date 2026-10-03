using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] AudioClip pageFlipSFX;

    public void PlayPageFlipSFX()
    {
        AudioBus.Instance.PlayOneshotSFX(pageFlipSFX);
    }
}
