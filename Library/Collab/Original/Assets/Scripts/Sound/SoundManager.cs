using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource effectSource;
    public AudioSource bgmSource;
    public AudioSource loopEffectsource;
    public static SoundManager instance = null;

    public float lowPichRange = .95f;
    public float highPitchRange = 1.05f;

    public bool isBgmOff = false;
    public bool isEffectOff = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

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
    public void BgmSourceChange(AudioClip clip)
    {
        if (!isEffectOff)
        {
            if (clip != null)
            {
                bgmSource.clip = clip;
                bgmSource.Play();
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
