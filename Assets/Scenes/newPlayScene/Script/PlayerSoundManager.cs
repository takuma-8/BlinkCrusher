using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private AudioSource footStepSource;
    private AudioSource breakObjectSource;
    private AudioSource lockerSource;


    [Header("Audio Clips")]
    public AudioClip footStepSound;     // ���s��
    public AudioClip breakTuboSound;  // ����󂷉�
    public AudioClip breakDozoSound;    // �������󂷉�
    public AudioClip lockerSound; // ���b�J�[���J���鉹

    void Start()
    {
        // �����p AudioSource
        footStepSource = gameObject.AddComponent<AudioSource>();
        footStepSource.loop = false;

        // �󂷉��p AudioSource
        breakObjectSource = gameObject.AddComponent<AudioSource>();
        breakObjectSource.loop = false;

        // ���b�J�[�J�p AudioSource
        lockerSource = gameObject.AddComponent<AudioSource>();
        lockerSource.loop = false;
    }

    // �����̍Đ�
    public void PlayFootStep()
    {
        if (footStepSource != null && !footStepSource.isPlaying)
        {
            footStepSource.PlayOneShot(footStepSound);
        }
    }

    // �����󂷉��̍Đ� (��ނ��w��)
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

    // �����̒�~
    public void StopFootStep()
    {
        if (footStepSource != null && footStepSource.isPlaying)
        {
            footStepSource.Stop();
        }
    }

    // ���b�J�[���J���鉹**
    public void PlayLocker()
    {
        if(lockerSource != null)
        lockerSource.PlayOneShot(lockerSound);
    }
}
