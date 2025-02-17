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
    [SerializeField] private float maxHearDistance = 100f; // 足音が聞こえる最大距離
    [SerializeField] private float minHearDistance = 1f;  // 最大音量になる距離


    void Start()
    {
        // プレイヤーのTransformを取得
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Playerタグを持つオブジェクトが見つかりません！");
            return;
        }

        // AudioSourceの初期設定
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

        Debug.Log($"maxHearDistance 初期化: {maxHearDistance}");
    }

    void Update()
    {
        Debug.Log($"足音設定 - maxDistance: {footstepSource.maxDistance}, minDistance: {footstepSource.minDistance}");
        Debug.Log($"プレイヤーとの距離: {Vector3.Distance(transform.position, player.position)}, maxHearDistance: {maxHearDistance}");
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
            if (!chaseSource.isPlaying) // すでに再生中なら何もしない
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
            Debug.Log("音をフェードアウト"); // 確認用
            StartCoroutine(FadeOutAndStop()); // 音をフェードアウト
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
        float fadeTime = 1.0f; // 1秒かけてフェードアウト
        float startVolume = chaseSource.volume;

        while (chaseSource.volume > 0)
        {
            chaseSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        chaseSource.Stop();
        chaseSource.volume = startVolume; // 次回再生時のために音量を元に戻す
        Debug.Log("音が完全に停止"); // 確認用
    }

    // ステレオパンをプレイヤーの左右位置に基づいて設定
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

        // **音量が0なら再生しない**
        if (footstepSource.volume <= 0f)
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
                Debug.Log("足音を停止（音量ゼロ）");
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
            Debug.LogWarning("足音クリップが設定されていません！");
            return;
        }

        footstepSource.Play();
        Debug.Log($"足音再生: {footstepSource.clip.name}, 音量: {footstepSource.volume}");
    }


    // 追跡開始（足音変更）
    public void SetChaseMode(bool chase)
    {
        isChasing = chase;
    }

    private void AdjustFootstepVolume()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log($"プレイヤーとの距離: {distance}, maxHearDistance: {maxHearDistance}");

        if (distance >= maxHearDistance)
        {
            if (footstepSource.isPlaying) // もし再生中なら停止
            {
                footstepSource.Stop();
                Debug.Log("足音を停止しました（範囲外）");
            }
            footstepSource.volume = 0f;
            return; // ここで処理を終了
        }

        // 音量を計算
        float volume = Mathf.Clamp01(1.0f - (distance - minHearDistance) / (maxHearDistance - minHearDistance));

        // もし音量が 0 になったら停止（念のため）
        if (volume <= 0f)
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
                Debug.Log("足音を停止（音量ゼロ）");
            }
            return;
        }

        footstepSource.volume = volume;
        Debug.Log($"足音音量: {footstepSource.volume}");
    }




}