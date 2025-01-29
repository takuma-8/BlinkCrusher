using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{

    private AudioSource chaseSource;

    [Header("Audio Clips")]
    public AudioClip chaseSound;    // 追跡中サウンド

    void Start()
    {
        // 追跡中用 AudioSource
        chaseSource = gameObject.AddComponent<AudioSource>();
        chaseSource.loop = false;
    }

    public void PlayChaseStart()
    {
        if(chaseSource != null)
        {
            chaseSource.PlayOneShot(chaseSound);
        }
    }

    public void StopChaseEnd()
    {
        if (chaseSource != null)
        {
            chaseSource.Stop();
        }
    }
}
