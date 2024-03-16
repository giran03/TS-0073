using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Configs")]
    [SerializeField] Sound[] musicSounds, sfxSounds;
    [SerializeField] AudioSource musicSource, sfxSource;

    public static AudioManager Instance;
    AudioSource currentSfxLooping;
    AudioSource currentMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start() => LevelAmbiance();

    public void LevelAmbiance()
    {
        if (LevelSceneManager.Instance.ActiveScene() == "002_LV1")
            PlayMusic("Forest");
        else if (LevelSceneManager.Instance.ActiveScene() == "003_LV2")
            PlayMusic("Desert_lvl2");
        else if (LevelSceneManager.Instance.ActiveScene() == "004_LV3")
            PlayMusic("Desert_lvl3");
        else if (LevelSceneManager.Instance.ActiveScene() == "005_LV4")
            PlayMusic("PostApocalypse");
    }

    public void PlayMusic(string clipName)
    {
        Sound sound = Array.Find(musicSounds, x => x.clipName == clipName);
        if (sound != null)
        {
            musicSource.clip = sound.audioClip;
            musicSource.loop = true;
            musicSource.Play();
            musicSource.volume = .6f;
            currentMusic = musicSource;
        }
    }

    public void PlaySFX(string clipName, Vector3 position, bool loop = false)
    {
        Sound sound = Array.Find(sfxSounds, x => x.clipName == clipName);
        if (sound != null)
        {
            sfxSource.loop = loop;

            if (loop)
            {
                if (currentSfxLooping != null)
                    currentSfxLooping.Stop();

                sfxSource.clip = sound.audioClip;
                sfxSource.Play();
                currentSfxLooping = sfxSource;
            }
            else
            {
                AudioSource.PlayClipAtPoint(sound.audioClip, position);
                StartCoroutine(Cooldown(sound.audioClip));
            }
        }
    }

    public void StopMusic()
    {
        if (currentMusic != null)
        {
            currentMusic.Stop();
            currentMusic = null;
        }
    }

    public void StopSFX()
    {
        if (currentSfxLooping != null)
        {
            currentSfxLooping.Stop();
            currentSfxLooping = null;
        }
    }

    IEnumerator Cooldown(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
    }
}
