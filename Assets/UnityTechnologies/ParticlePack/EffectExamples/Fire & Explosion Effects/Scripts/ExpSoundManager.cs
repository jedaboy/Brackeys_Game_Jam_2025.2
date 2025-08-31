using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpSoundManager : SoundManager
{
    [SerializeField] private AudioClip[] explosion;
    private void OnEnable()
    {
        PlaySound(explosion[Random.Range(0, explosion.Length)]);
    }
}
