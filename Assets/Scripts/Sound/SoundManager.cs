using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource effectSource;
    public AudioSource bgmSource;
    public AudioSource loopEffectsource;
    private static SoundManager _instance = null;
    public static SoundManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;

                if (_instance == null)
                {
                    Debug.LogError("There's no active SoundManager object");
                }
            }

            return _instance;
        }
    }

    public float lowPichRange = .97f;
    public float highPitchRange = 1f;

    public bool isBgmOff = false;
    public bool isEffectOff = false;



    private void Start()
    {
        StartBGM();
    }
    IEnumerator StartBgmVolume()
    {
        bgmSource.volume = 0;
        while(bgmSource.volume<1)
        {
            bgmSource.volume += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        bgmSource.volume = 1;
        yield return null;
    }
    IEnumerator StopBgmVolume()
    {
        bgmSource.volume = 1;
        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        bgmSource.volume = 0;
        yield return null;
    }
    public void StartBGM()
    {
        if(!isBgmOff)
            StartCoroutine("StartBgmVolume");
    }
    public void BgmOnOff(bool isOff)
    {
        isBgmOff = isOff;
        if(isBgmOff)
            StartCoroutine("StopBgmVolume");
        else
            StartCoroutine("StartBgmVolume");
    }
    public void EffectOnOff(bool isOff)
    {
        isEffectOff = isOff;
    }

    public void PlaySingle(AudioClip clip)
    {
        if(!isEffectOff)
        {
            if (clip != null)
            {
                EffectSourcePlay(clip);
            }
        }
    }
    public void EffectSourcePlay(params AudioClip[] clips)
    {
        if(!isEffectOff)
        {
            int randomIndex = Random.Range(0, clips.Length);
            float randomPitch = Random.Range(lowPichRange, highPitchRange);
            effectSource.pitch = randomPitch;
            if (clips[randomIndex] != null)
                effectSource.PlayOneShot(clips[randomIndex]);
        }
    }
    public void EffectSourcePlayNoPitch(AudioClip clip)
    {
        if (!isEffectOff)
        {
            effectSource.pitch = 1;
            effectSource.PlayOneShot(clip);
        }
    }
    public void BgmSourceChange(AudioClip clip)
    {
        if (!isBgmOff)
        {
            if (clip != null)
            {
                bgmSource.clip = clip;
                bgmSource.Play();
            }
            else
            {
                bgmSource.Stop();
            }

        }
    }
    public void PlaySingleLoop(AudioClip clip)
    {
        if(!isEffectOff)
        {
            if (clip != null)
            {
                loopEffectsource.clip = clip;
                loopEffectsource.Play();
            }
        }
    }

    public void StopSingleLoop()
    {
        loopEffectsource.clip = null;
        loopEffectsource.Stop();
    }
}
