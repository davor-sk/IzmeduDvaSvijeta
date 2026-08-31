using UnityEngine;

public class MainMenuAudioController : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip backgroundLoop;
    public AudioClip buttonClickSfx;

    [Range(0f, 1f)] public float musicVolume = 0.4f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    void Start()
    {
        if (musicSource != null && backgroundLoop != null)
        {
            musicSource.clip = backgroundLoop;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void PlayButtonClick()
    {
        if (sfxSource != null && buttonClickSfx != null)
        {
            sfxSource.PlayOneShot(buttonClickSfx, sfxVolume);
        }
    }
}