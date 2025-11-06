using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private void Awake()
    {
        instance = this;
    }

    public AudioSource[] soudEffects;

    public void PlaySFX(int sfxToPlay)
    {
        soudEffects[sfxToPlay].Stop();
        soudEffects[sfxToPlay].Play();
    }

    public void PlaySFXPitched(int sfxToPlay)
    {
        soudEffects[sfxToPlay].pitch = Random.Range(.8f, 1.2f);

        PlaySFX(sfxToPlay);
    }
}
