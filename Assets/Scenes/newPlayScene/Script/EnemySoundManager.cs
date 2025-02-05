using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    public AudioSource chaseSource;
    public AudioSource footstepSource;
    private Transform player;  // Reference to the player's transform

    [Header("Audio Clips")]
    public AudioClip chaseSound;    // Chase sound
    public AudioClip walkSound;
    public AudioClip runSound;

    public float stepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    [Header("Sound Settings")]
    public float maxDistance = 20f;  // Max distance at which the sound is fully heard
    public float minDistance = 5f;   // Min distance at which the sound is at its loudest
    public float minPitch = 1f;      // Minimum pitch (when the player is far)
    public float maxPitch = 2f;      // Maximum pitch (when the player is close)
    public float maxPanRange = 1f;

    private float nextStepTime = 0f;
    private bool isChasing = false;

    [Header("Sound Distance Settings")]
    public float maxHearDistance = 100f; // ��������������ő勗��
    public float minHearDistance = 1f;  // �ő剹�ʂɂȂ鋗��


    void Start()
    {
        // Get the player's transform
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if(player == null )
        {
            Debug.LogWarning("Player�^�O�����I�u�W�F�N�g��������܂���I");
            return;
        }

        // Create AudioSource for chase sound
        chaseSource = gameObject.AddComponent<AudioSource>();
        chaseSource.loop = true;
        chaseSource.spatialBlend = 0;

        // �����p��AudioSource���쐬
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = false;  // ���[�v���Ȃ�
        footstepSource.playOnAwake = false;

        footstepSource.spatialBlend = 1.0f; // 0 = 2D, 1 = 3D
        footstepSource.rolloffMode = AudioRolloffMode.Linear; // �����ɂ�鉹�ʒ���
        footstepSource.maxDistance = maxHearDistance; // ����ȏ㗣���ƕ������Ȃ��Ȃ�
        footstepSource.minDistance = minHearDistance; // �߂��ōő剹��
        footstepSource.volume = 1.0f;
    }

    void Update()
    {
        if (chaseSource.isPlaying)
        {
            AdjustSoundPropertiesBasedOnDistance();
            AdjustStereoPan();
        }
        AdjustFootstepVolume();
        PlayFootstepSound();
    }

    public void PlayChaseStart()
    {
        if (chaseSource != null && chaseSound != null)
        {
            if (!chaseSource.isPlaying) // ���łɍĐ����Ȃ牽�����Ȃ�
            {
                chaseSource.clip = chaseSound;
                chaseSource.loop = true;
                chaseSource.Play();
            }
        }
    }

    public void StopChaseEnd()
    {
        if (chaseSource != null && chaseSource.isPlaying)
        {
            Debug.Log("�����t�F�[�h�A�E�g"); // �m�F�p
            StartCoroutine(FadeOutAndStop()); // �����t�F�[�h�A�E�g
        }
    }

    // Adjust the volume and pitch based on the distance to the player
    private void AdjustSoundPropertiesBasedOnDistance()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Adjust volume
            float volume = Mathf.Clamp01(1 - (distance - minDistance) / (maxDistance - minDistance));
            chaseSource.volume = volume;

            // Adjust pitch
            float pitch = Mathf.Lerp(maxPitch, minPitch, (distance - minDistance) / (maxDistance - minDistance));
            chaseSource.pitch = pitch;
        }
    }

    private IEnumerator FadeOutAndStop()
    {
        float fadeTime = 1.0f; // 1�b�����ăt�F�[�h�A�E�g
        float startVolume = chaseSource.volume;

        while (chaseSource.volume > 0)
        {
            chaseSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        chaseSource.Stop();
        chaseSource.volume = startVolume; // ����Đ����̂��߂ɉ��ʂ����ɖ߂�
        Debug.Log("�������S�ɒ�~"); // �m�F�p
    }

    // �X�e���I�p�����v���C���[�̍��E�ʒu�Ɋ�Â��Đݒ�
    private void AdjustStereoPan()
    {
        if (player != null)
        {
            float relativeX = transform.position.x - player.position.x;
            float pan = Mathf.Clamp(relativeX / maxDistance, -maxPanRange, maxPanRange);
            chaseSource.panStereo = pan;
        }
    }

    private void PlayFootstepSound()
    {
        if (Time.time >= nextStepTime && footstepSource != null)
        {
            if (isChasing && runSound != null)
            {
                footstepSource.clip = runSound;
                nextStepTime = Time.time + runStepInterval; // �ǐՎ��̊Ԋu
            }
            else if (!isChasing && walkSound != null)
            {
                footstepSource.clip = walkSound;
                nextStepTime = Time.time + stepInterval; // �ʏ펞�̊Ԋu
            }
            if (footstepSource.clip == null)
            {
                Debug.LogWarning("�����N���b�v���ݒ肳��Ă��܂���I");
                return;
            }
            footstepSource.Play();
            Debug.Log($"�����Đ�: {footstepSource.clip.name}, ����: {footstepSource.volume}");
        }
    }

    // �ǐՊJ�n�i�����ύX�j
    public void SetChaseMode(bool chase)
    {
        isChasing = chase;
    }

    private void AdjustFootstepVolume()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > maxHearDistance)
        {
            footstepSource.volume = 0f; // ��苗���𒴂����犮�S�ɕ������Ȃ�����
        }
        else
        {
            float minFootstepVolume = 0.2f; // �Œቹ�ʂ�ݒ�
            float volume = Mathf.Clamp(minFootstepVolume + (1 - minFootstepVolume) * (1 - (distance - minHearDistance) / (maxHearDistance - minHearDistance)), minFootstepVolume, 1.0f);
            footstepSource.volume = volume;
        }
    }
}