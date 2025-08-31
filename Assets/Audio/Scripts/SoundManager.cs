using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;

    public void PlaySound(AudioClip clip)
    {
        m_AudioSource.PlayOneShot(clip);
    }
}
