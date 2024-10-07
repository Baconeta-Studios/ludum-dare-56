using UnityEngine;
using Utils;

public class AudioSystem : Singleton<AudioSystem>
{
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, float volume=1f)
    {
         audioSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip, float volume=1f)
    {
        var music = gameObject.AddComponent<AudioSource>();
        music.clip = clip;
        music.loop = true;
        music.volume = volume;
        music.Play();
    }
}
