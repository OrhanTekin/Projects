using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum Sound
    {
        None,
        ShootingSound,
        PlayerBulletImpactSound,
        EnemyBulletImpactSound,
        LaserSound,      //staubsauger sound
        RocketShootSound, //When rockets or missiles are fired          
        LanceRotateSound, 
        RevolverSound,
        RevolverHitSound,
        FinalBossEnterSound, 
        FinalBossStartHealSound,
        FinalBossHealSound,
    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.ShootingSound] = 0f;
        soundTimerDictionary[Sound.PlayerBulletImpactSound] = 0f;
        soundTimerDictionary[Sound.LanceRotateSound] = 0f;
        soundTimerDictionary[Sound.FinalBossHealSound] = 0f;
        soundTimerDictionary[Sound.RevolverHitSound] = 0f;
        soundTimerDictionary[Sound.EnemyBulletImpactSound] = 0f;
        soundTimerDictionary[Sound.RocketShootSound] = 0f;
        soundTimerDictionary[Sound.LaserSound] = 0f;
    }

    public static void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            if(oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }


    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.None:
                return false;
            case Sound.ShootingSound:
                return CanPlayCheckKeyAndTimer(sound, 0.25f);              
            case Sound.PlayerBulletImpactSound:
                return CanPlayCheckKeyAndTimer(sound, 0.1f);
            case Sound.LanceRotateSound:
                return CanPlayCheckKeyAndTimer(sound, 1.2f);
            case Sound.FinalBossHealSound:
                return CanPlayCheckKeyAndTimer(sound, 0.35f);
            case Sound.RevolverHitSound:
                return CanPlayCheckKeyAndTimer(sound, 1f);
            case Sound.EnemyBulletImpactSound:
                return CanPlayCheckKeyAndTimer(sound, 0.2f);
            case Sound.RocketShootSound:
                return CanPlayCheckKeyAndTimer(sound, 0.2f);
            case Sound.LaserSound:
                return CanPlayCheckKeyAndTimer(sound, 5f);
        }
    }

    private static bool CanPlayCheckKeyAndTimer(Sound sound, float delay)
    {
        if (soundTimerDictionary.ContainsKey(sound))
        {
            if (soundTimerDictionary[sound] + delay < Time.time)
            {
                soundTimerDictionary[sound] = Time.time;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
            
    }


    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach(GameManager.SoundAudioClip soundAudioClip in GameManager.Instance.soundAudioClips)
        {
            if(soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + "not found!");
        return null;
    }

}

//In any class write this
//SoundManager.PlaySound(SoundManager.Sound.ImpactSound);
