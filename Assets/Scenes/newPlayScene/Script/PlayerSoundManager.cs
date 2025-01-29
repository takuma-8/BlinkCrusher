using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private AudioSource footStepSource;
    private AudioSource breakObjectSource;
    private AudioSource lockerSource;


    [Header("Audio Clips")]
    public AudioClip footStepSound;     // 歩行音
    public AudioClip breakTuboSound;  // 壺を壊す音
    public AudioClip breakDozoSound;    // 銅像を壊す音
    public AudioClip lockerSound; // ロッカーを開ける音

    void Start()
    {
        // 足音用 AudioSource
        footStepSource = gameObject.AddComponent<AudioSource>();
        footStepSource.loop = false;

        // 壊す音用 AudioSource
        breakObjectSource = gameObject.AddComponent<AudioSource>();
        breakObjectSource.loop = false;

        // ロッカー開閉用 AudioSource
        lockerSource = gameObject.AddComponent<AudioSource>();
        lockerSource.loop = false;
    }

    // 足音の再生
    public void PlayFootStep()
    {
        if (footStepSource != null && !footStepSource.isPlaying)
        {
            footStepSource.PlayOneShot(footStepSound);
        }
    }

    // 物を壊す音の再生 (種類を指定)
    public void PlayBreakSound(string objectType)
    {
        if (breakObjectSource != null)
        {
            if (objectType == "cap" && breakTuboSound != null)
            {
                breakObjectSource.PlayOneShot(breakTuboSound);
            }
            else if (objectType == "kabin" && breakDozoSound != null)
            {
                breakObjectSource.PlayOneShot(breakDozoSound);
            }
        }
    }

    // 足音の停止
    public void StopFootStep()
    {
        if (footStepSource != null && footStepSource.isPlaying)
        {
            footStepSource.Stop();
        }
    }

    // ロッカーを開ける音**
    public void PlayLocker()
    {
        if(lockerSource != null)
        lockerSource.PlayOneShot(lockerSound);
    }
}
