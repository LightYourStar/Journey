using System.Collections.Generic;
using JO;
using UnityEngine;

public class SoundMgr
{

    private static SoundMgr _instance;
    public static SoundMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SoundMgr();
            }
            return _instance;
        }
    }

    public SoundMgr()
    {

    }

    private string bgmName = string.Empty;


    private AudioSource bgmAudioSource = null;

    private AudioSource BgmAudioSource
    {
        get
        {
            if (bgmAudioSource == null)
            {
                GameObject bgmObj = new GameObject("BgmAudioSource");
                bgmAudioSource = bgmObj.AddComponent<AudioSource>();
                bgmAudioSource.loop = true;
                Object.DontDestroyOnLoad(bgmObj);
            }
            return bgmAudioSource;
        }
    }

    private AudioSource sound = null;

    private AudioSource Sound
    {
        get
        {
            if (sound == null)
            {
                GameObject soundObj = new GameObject("SoundAudioSource");
                sound = soundObj.AddComponent<AudioSource>();
                Object.DontDestroyOnLoad(soundObj);
            }
            return sound;
        }
    }

    public static void PlayBgm(string name)
    {
        Instance._PlayBgm(name);
    }

    private void _PlayBgm(string name)
    {
        if (bgmName == name)
        {
            return;
        }
        bgmName = name;
        AudioClip clip = Global.gApp.gResMgr.LoadAssets<AudioClip>($"MP3/Gamejam/{name}.mp3", ResType.Clip);
        BgmAudioSource.clip = clip;
        BgmAudioSource.Play();
    }

    public static void PlaySound(string name)
    {
        Instance._PlaySound(name);
    }

    private void _PlaySound(string name)
    {
        AudioClip clip = Global.gApp.gResMgr.LoadAssets<AudioClip>($"MP3/Gamejam/{name}.mp3", ResType.Clip);
        Sound.PlayOneShot(clip);
    }
}