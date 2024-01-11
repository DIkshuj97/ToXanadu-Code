using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Sound[] allSfx;
    public Sound[] allMusic;

    [SerializeField] private AudioSource EffectsSource;
    [SerializeField] private AudioSource MusicSource;

    public static SoundManager ins;
    void Awake()
    {
        if (ins != null && ins != this)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        SetAllSfx();
        SetAllMusic();
        PlayMusic("BGM");
    }
   
    void SetAllSfx()
    {
        foreach (Sound sound in allSfx)
        {
            sound.source = EffectsSource; //gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }
    void SetAllMusic()
    {
        foreach (Sound sound in allMusic)
        {
            sound.source = MusicSource; //gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    void SetMusicandSfx(Sound sound, AudioSource _aS)
    {
        
        sound.source = _aS;
        _aS.clip = sound.clip;
        _aS.volume = sound.volume;
        _aS.pitch = sound.pitch;
        _aS.loop = sound.loop;
    }
    
    public void StopSfx()
    {
        EffectsSource.Stop();
    }
    public void StopMusic()
    {
        MusicSource.Stop();
    }
    
    public void ClickSFX()
    {
        PlaySfx("ButtonClick");
    }

    public void PlaySfx(string name)
    {
        Sound snd = Array.Find(allSfx, sound => sound.name == name);
        SetMusicandSfx(snd, EffectsSource);
        try
        {
            snd.source.Play();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e+" sfx not found");
        }
    }

    public void PlayMusic(string name)
    {
        Sound snd = Array.Find(allMusic, sound => sound.name == name);
        if (snd != null)
        {
            SetMusicandSfx(snd, MusicSource);
            try
            {
                snd.source.Play();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e + " music not found");
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("Sound with name: " + name + " not found");
            }
            else
            {
                Debug.LogWarning("not valid name for audio clip");
            }
        }
    }
}
