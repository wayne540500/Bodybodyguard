using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public enum SoundType
    {

        TeleportPipelineSound,
        shoooterFire,
        laserFire,
        enemyDead,
        enemyShoot,
        shipDamaged_Large,
        shipDamaged_Medium,
        shipDamaged_Little,
        shipHeal,
        shieldHeal,
        shieldHit,
        MapOpen,
        MapClose,
        booster,
        missle,
        explosion,
        WrenchPickUp,
        bossEggExplode,
        bossLaserBallCharge,
        bossLaserCharge,
        bossEvolve,
        underWaterBombCountDown,
        underWaterBombExplosion,
        shipExplode,
        shipLaserReady,
        UpgradeMenuOpen,
        shipUpgrade,
        boostMode,
        phoenixMode,
        ShieldBroken,


    }

    [System.Serializable]
    public class SoundClip
    {
        public bool isMusic;
        public SoundType soundType;
        public AudioClip audioClip;
        public bool isLoop;
        [Range(.1f, 3f)]
        public float pitch;
        [Range(0f, 1f)]
        public float volume;
        [Range(0, 256)]
        public int priority;

        [HideInInspector]
        public AudioSource audioSource;

    }

    public static SoundManager Instance;
    public AudioMixerGroup music;
    public AudioMixerGroup sfx;
    public SoundClip[] soundClips;

    private void Awake()
    {
        Instance = this;
        foreach (SoundClip clip in soundClips)
        {
            clip.audioSource = gameObject.AddComponent<AudioSource>();
            clip.audioSource.playOnAwake = false;
            clip.audioSource.outputAudioMixerGroup = clip.isMusic ? music : sfx;
            clip.audioSource.clip = clip.audioClip;
            clip.audioSource.loop = clip.isLoop;
            clip.audioSource.priority = clip.priority;
            clip.audioSource.pitch = clip.pitch;
            clip.audioSource.volume = clip.volume;
        }
    }

    // private void Update()
    // {
    //     if (Time.timeScale == 0f)
    //     {
    //         for (int i = 0; i < soundClips.Length; i++)
    //         {
    //             if (soundClips[i].audioSource.isPlaying)
    //                 StopPlaySound(soundClips[i].soundType);
    //         }
    //     }
    // }

    public void PlaySoundOneShot(SoundType sound, bool isIgnoreTime)
    {
        if (Time.timeScale == 0 && !isIgnoreTime)
            return;
        AudioSource audioSource = GetAudioSource(sound);
        audioSource.PlayOneShot(audioSource.clip);
    }

    public void PlaySoundLoop(SoundType sound, bool isIgnoreTime)
    {
        if (Time.timeScale == 0 && !isIgnoreTime)
        {
            StopPlaySound(sound);
            return;
        }
        AudioSource audioSource = GetAudioSource(sound);
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void PlaySound(SoundType sound, bool isIgnoreTime)
    {
        if (Time.timeScale == 0 && !isIgnoreTime)
        {
            StopPlaySound(sound);
            return;
        }
        AudioSource audioSource = GetAudioSource(sound);
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopPlaySound(SoundType sound)
    {
        AudioSource audioSource = GetAudioSource(sound);
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    public AudioSource GetAudioSource(SoundType sound)
    {
        foreach (var soundClip in soundClips)
        {
            if (soundClip.soundType == sound)
                return soundClip.audioSource;
        }

        Debug.LogError($"audioSource '{sound.ToString()}' not found");
        return null;
    }

}
