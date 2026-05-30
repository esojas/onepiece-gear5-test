//Credit to: Small Hedge Games

using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SmallHedge.SoundManager
{
    public enum SoundType
    {
        Footstep,
        Punch,
        Kick,
        EnemyMelee,
        EnemyShoot,
        Jump,
        Hurt,
        ThrowThunder,
        Thunder,
        ThunderHold,
        Grapple,
        ThrowArm,
        ReturnArm
    }

    public enum MusicType
    {
        MainMenu,
        Gameplay,
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundsSO SO;
        [SerializeField] private AudioSource musicSource;
        private static SoundManager instance = null;
        private AudioSource audioSource;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                audioSource = GetComponent<AudioSource>();
                DontDestroyOnLoad(gameObject);

                musicSource.loop = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void PlayMusic(MusicType music, float volume = 1f)
        {
            SoundList musicList = instance.SO.music[(int)music];
            AudioClip[] clips = musicList.sounds;
            AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

            instance.musicSource.outputAudioMixerGroup = musicList.mixer;
            instance.musicSource.clip = randomClip;
            instance.musicSource.volume = volume * musicList.volume;
            instance.musicSource.Play();
        }

        public static void StopMusic() => instance.musicSource.Stop();
        public static void PauseMusic() => instance.musicSource.Pause();
        public static void ResumeMusic() => instance.musicSource.UnPause();

        private static AudioClip GetRandomClip(SoundList soundList)
        {
            return soundList.sounds[UnityEngine.Random.Range(0, soundList.sounds.Length)];
        }

        private static float GetPitch(SoundList soundList)
        {
            return soundList.randomizePitch
                ? UnityEngine.Random.Range(soundList.minPitch, soundList.maxPitch)
                : 1f;
        }

        public static void PlayLoopingSound(SoundType sound, AudioSource source, float volume = 1f)
        {
            SoundList soundList = instance.SO.sounds[(int)sound];

            source.outputAudioMixerGroup = soundList.mixer;
            source.pitch = GetPitch(soundList);
            source.clip = GetRandomClip(soundList);
            source.volume = volume * soundList.volume;
            source.loop = true;
            source.Play();
        }

        public static void PlaySoundAtPosition(SoundType sound, Vector3 position, float volume = 1f, float minimalDistance = 1f)
        {
            SoundList soundList = instance.SO.sounds[(int)sound];
            AudioClip randomClip = GetRandomClip(soundList);

            GameObject tempGO = new GameObject($"TempAudio_{sound}");
            tempGO.transform.position = position;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.outputAudioMixerGroup = soundList.mixer;
            tempSource.clip = randomClip;
            tempSource.volume = volume * soundList.volume;
            tempSource.pitch = GetPitch(soundList);
            tempSource.spatialBlend = 1f;
            tempSource.minDistance = minimalDistance;
            tempSource.Play();

            Destroy(tempGO, randomClip.length);
        }

        public static void StopLoopingSound(AudioSource source)
        {
            source.loop = false;
            source.Stop();
        }

        public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1f)
        {
            SoundList soundList = instance.SO.sounds[(int)sound];
            AudioClip randomClip = GetRandomClip(soundList);
            float pitch = GetPitch(soundList);

            if (source)
            {
                source.outputAudioMixerGroup = soundList.mixer;
                source.pitch = pitch;
                source.clip = randomClip;
                source.volume = volume * soundList.volume;
                source.Play();
            }
            else
            {
                instance.audioSource.outputAudioMixerGroup = soundList.mixer;
                instance.audioSource.pitch = pitch;
                instance.audioSource.PlayOneShot(randomClip, volume * soundList.volume);
            }
        }
    }

    [Serializable]
    public struct SoundList
    {
        [HideInInspector] public string name;
        [Range(0, 1)] public float volume;
        public AudioMixerGroup mixer;
        public AudioClip[] sounds;

        public bool randomizePitch;
        [Range(0.5f, 2f)] public float minPitch;
        [Range(0.5f, 2f)] public float maxPitch;
    }
}