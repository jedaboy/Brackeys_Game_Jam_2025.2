using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : SoundManager
{
    
    [SerializeField] private AudioClip[] _shootSound;
    public AudioClip shootSound => _shootSound[Random.Range(0, _shootSound.Length)];
    [SerializeField] private AudioClip[] _damage;
    public AudioClip damageSound => _damage[Random.Range(0, _damage.Length)];

    [SerializeField] private AudioClip[] _explosion;
    public AudioClip explosionSound => _explosion[Random.Range(0, _explosion.Length)];

}
