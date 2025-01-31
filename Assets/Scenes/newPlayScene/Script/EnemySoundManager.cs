using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    public AudioSource chaseSource;
    private Transform player;  // Reference to the player's transform

    [Header("Audio Clips")]
    public AudioClip chaseSound;    // Chase sound

    [Header("Sound Settings")]
    public float maxDistance = 20f;  // Max distance at which the sound is fully heard
    public float minDistance = 5f;   // Min distance at which the sound is at its loudest
    public float minPitch = 1f;      // Minimum pitch (when the player is far)
    public float maxPitch = 2f;      // Maximum pitch (when the player is close)

    void Start()
    {
        // Get the player's transform
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Create AudioSource for chase sound
        chaseSource = gameObject.AddComponent<AudioSource>();
        chaseSource.loop = true;
    }

    void Update()
    {
        if (chaseSource.isPlaying)
        {
            AdjustSoundPropertiesBasedOnDistance();
        }
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
}
