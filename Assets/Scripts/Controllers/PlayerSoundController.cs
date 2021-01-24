using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de audio del personaje jugable
/// Contiene métodos para reproducir los sonidos de forma sencilla
/// </summary>
public class PlayerSoundController : MonoBehaviour
{
    #region VARIABLES
    // Referencias a los audios
    public AudioClip pistol;
    public AudioClip rifle;
    public AudioClip flamethrower;
    public AudioClip grenadeLauncher;
    public AudioClip sniper;

    public List<AudioClip> damage;
    public AudioClip death;

    // Ajustes de volumen
    private float pistolVolume = 0.3f;
    private float rifleVolume = 1f;
    private float flamethrowerVolume = 0.5f;
    private float grenadeLauncherVolume = 0.7f;
    private float sniperVolume = 0.7f;
    private float damageVolume = 1f;
    private float deathVolume = 1f;

    // Referencias privadas
    private AudioSource weaponsAudioSource;
    private AudioSource characterAudioSource;

    // Datos
    private bool isFlamethrowerPlaying;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        weaponsAudioSource = GetComponents<AudioSource>()[0];
        characterAudioSource = GetComponents<AudioSource>()[1];
        isFlamethrowerPlaying = false;
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Reproduce el sonido asociado al arma
    /// El sonido de armas continuas (flamethrower) debe llamar a StopWeaponShot
    /// </summary>
    /// <param name="weapon"></param>
    public void PlayWeaponShot(Weapons weapon)
    {
        switch(weapon)
        {
            case Weapons.Pistol:
                weaponsAudioSource.PlayOneShot(pistol, pistolVolume);
                break;

            case Weapons.Rifle:
                weaponsAudioSource.PlayOneShot(rifle, rifleVolume);
                break;

            case Weapons.Flamethrower:
                if (!isFlamethrowerPlaying)
                {
                    isFlamethrowerPlaying = true;
                    weaponsAudioSource.loop = true;
                    weaponsAudioSource.clip = flamethrower;
                    weaponsAudioSource.volume = flamethrowerVolume;
                    weaponsAudioSource.Play();
                }
                break;

            case Weapons.GrenadeLauncher:
                weaponsAudioSource.PlayOneShot(grenadeLauncher, grenadeLauncherVolume);
                break;

            case Weapons.Sniper:
                weaponsAudioSource.PlayOneShot(sniper, sniperVolume);
                break;
        }
    }

    /// <summary>
    /// Detiene el sonido asociado al arma (si es continua)
    /// </summary>
    /// <param name="weapon"></param>
    public void StopWeaponShot(Weapons weapon)
    {
        switch (weapon)
        {
            case Weapons.Flamethrower:
                isFlamethrowerPlaying = false;
                weaponsAudioSource.loop = false;
                weaponsAudioSource.clip = null;
                weaponsAudioSource.volume = 1.0f;
                weaponsAudioSource.Stop();
                break;
        }
    }

    /// <summary>
    /// Reproduce un sonido de daño aleatorio
    /// </summary>
    public void PlayDamageSound()
    {
        if (!characterAudioSource.isPlaying)
        {
            AudioClip sound = damage[Random.Range(0, damage.Count)]; ;
            characterAudioSource.clip = sound;
            characterAudioSource.volume = damageVolume;
            characterAudioSource.Play();
        }
    }

    /// <summary>
    /// Reproduce el sonido de muerte
    /// </summary>
    public void PlayDeathSound()
    {
        characterAudioSource.Stop();
        characterAudioSource.clip = death;
        characterAudioSource.volume = deathVolume;
        characterAudioSource.Play();
    }
    #endregion
}
