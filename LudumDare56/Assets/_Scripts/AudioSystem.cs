using UnityEngine;
using Utils;

public class AudioSystem : Singleton<AudioSystem>
{
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
         audioSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        var music = gameObject.AddComponent<AudioSource>();
        music.clip = clip;
        music.loop = true;
        music.Play();
    }
}
