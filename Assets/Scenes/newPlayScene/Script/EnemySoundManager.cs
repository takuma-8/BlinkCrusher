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
    public float maxDistance = 30f;  // Max distance at which the sound is fully heard
    public float minDistance = 5f;   // Min distance at which the sound is at its loudest
    public float minPitch = 1f;      // Minimum pitch (when the player is far)
    public float maxPitch = 2f;      // Maximum pitch (when the player is close)
    public float maxPanRange = 1f;

    private float nextStepTime = 0f;
    private bool isChasing = false;

    [Header("Sound Distance Settings")]
    [SerializeField] private float maxHearDistance = 100f; // ��������������ő勗��
    [SerializeField] private float minHearDistance = 1f;  // �ő剹�ʂɂȂ鋗��


    void Start()
    {
        // �v���C���[��Transform���擾
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player�^�O�����I�u�W�F�N�g��������܂���I");
            return;
        }

        // AudioSource�̏����ݒ�
        chaseSource = gameObject.AddComponent<AudioSource>();
        chaseSource.loop = true;
        chaseSource.spatialBlend = 0;

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = false;
        footstepSource.playOnAwake = false;
        footstepSource.spatialBlend = 1.0f;
        footstepSource.rolloffMode = AudioRolloffMode.Logarithmic;
        footstepSource.volume = 1.0f;

        maxHearDistance = 110f;

        footstepSource.maxDistance = maxHearDistance;
        footstepSource.minDistance = minHearDistance;

        Debug.Log($"maxHearDistance ������: {maxHearDistance}");
    }

    void Update()
    {
        Debug.Log($"�����ݒ� - maxDistance: {footstepSource.maxDistance}, minDistance: {footstepSource.minDistance}");
        Debug.Log($"�v���C���[�Ƃ̋���: {Vector3.Distance(transform.position, player.position)}, maxHearDistance: {maxHearDistance}");
        Debug.Log($"footstepSource.maxDistance: {footstepSource.maxDistance}, maxHearDistance: {maxHearDistance}"); if (chaseSource.isPlaying)
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
            float volume = Mathf.Clamp(
                minHearDistance + (1 - minHearDistance) * (1 - (distance - minHearDistance) / (maxHearDistance - minHearDistance)),
                minHearDistance,
                1.0f
            );


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
        if (Time.time < nextStepTime || footstepSource == null) return;

        // **���ʂ�0�Ȃ�Đ����Ȃ�**
        if (footstepSource.volume <= 0f)
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
                Debug.Log("�������~�i���ʃ[���j");
            }
            return;
        }

        if (isChasing && runSound != null)
        {
            footstepSource.clip = runSound;
            nextStepTime = Time.time + runStepInterval;
        }
        else if (!isChasing && walkSound != null)
        {
            footstepSource.clip = walkSound;
            nextStepTime = Time.time + stepInterval;
        }

        if (footstepSource.clip == null)
        {
            Debug.LogWarning("�����N���b�v���ݒ肳��Ă��܂���I");
            return;
        }

        footstepSource.Play();
        Debug.Log($"�����Đ�: {footstepSource.clip.name}, ����: {footstepSource.volume}");
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
        Debug.Log($"�v���C���[�Ƃ̋���: {distance}, maxHearDistance: {maxHearDistance}");

        if (distance >= maxHearDistance)
        {
            if (footstepSource.isPlaying) // �����Đ����Ȃ��~
            {
                footstepSource.Stop();
                Debug.Log("�������~���܂����i�͈͊O�j");
            }
            footstepSource.volume = 0f;
            return; // �����ŏ������I��
        }

        // ���ʂ��v�Z
        float volume = Mathf.Clamp01(1.0f - (distance - minHearDistance) / (maxHearDistance - minHearDistance));

        // �������ʂ� 0 �ɂȂ������~�i�O�̂��߁j
        if (volume <= 0f)
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
                Debug.Log("�������~�i���ʃ[���j");
            }
            return;
        }

        footstepSource.volume = volume;
        Debug.Log($"��������: {footstepSource.volume}");
    }




}