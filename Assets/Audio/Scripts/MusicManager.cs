using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip baseMusicClip;
    [SerializeField] private AudioClip expeditionMusicClip;
    [SerializeField] private AudioClip sentinelMusicClip;
    private bool musicOn;

    private void Update()
    {
        if (musicOn)
        {
            audioSource.volume += Time.deltaTime;
        }
        else
        {
            audioSource.volume -= Time.deltaTime;
        }

        audioSource.volume = Mathf.Clamp01(audioSource.volume);

    }
    private async void ChangeMusic(AudioClip newMusic)
    {       
        if(audioSource.clip == newMusic)
            return;
        await FadeOut();
        audioSource.Stop();
        audioSource.clip = newMusic;
        audioSource.Play();
        musicOn = true;
    }

    public void PlayBaseMusic()
    {
        ChangeMusic(baseMusicClip);
    }

    public void PlayExpeditionMusic()
    {
        ChangeMusic(expeditionMusicClip);
    }

    public void PlaySentinelMusic()
    {
        ChangeMusic(sentinelMusicClip);
    }

    public async Task FadeOut()
    {
        musicOn = false;
        while (audioSource.volume>0)
        {
            await Task.Delay(500);
        }
    }

}
