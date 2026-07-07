using UnityEngine;
using Yarn.Unity;

public class GameAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip radioStatic;
    [SerializeField] private AudioClip notificationBeep;
    [SerializeField] private AudioClip keyboardTyping;
    [SerializeField] private AudioClip keyboardClick;
    [SerializeField] private AudioClip cinematicHit;

    [YarnCommand("radio_static")]
    public void PlayRadioStatic()
    {
        PlaySFX(radioStatic);
    }

    [YarnCommand("notification_beep")]
    public void PlayNotificationBeep()
    {
        PlaySFX(notificationBeep);
    }

    [YarnCommand("keyboard_typing")]
    public void PlayKeyboardTyping()
    {
        PlaySFX(keyboardTyping);
    }

    [YarnCommand("keyboard_click")]
    public void PlayKeyboardClick()
    {
        PlaySFX(keyboardClick);
    }

    [YarnCommand("cinematic_hit")]
    public void PlayCinematicHit()
    {
        PlaySFX(cinematicHit);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}