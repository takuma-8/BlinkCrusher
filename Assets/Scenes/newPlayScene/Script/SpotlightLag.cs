using UnityEngine;

public class SpotlightLag : MonoBehaviour
{
    public Transform spotlight; // スポットライトのTransform
    public float lagSpeed = 2f; // 遅延の速度
    public float randomOffInterval = 0.1f; // ランダムでオフになるまでの平均間隔（秒）
    public float randomOffDuration = 0.1f; // ランダムでオフになる時間（秒）

    private Vector3 targetPosition;
    private Light spotlightLight; // スポットライトのLightコンポーネント
    private bool isSpotlightOn = true; // スポットライトのオンオフ状態
    private float nextRandomOffTime; // 次にランダムでオフになる時間

    void Start()
    {
        if (spotlight == null)
        {
            Debug.LogError("スポットライトが設定されていません！");
            enabled = false;
            return;
        }

        // Lightコンポーネントを取得
        spotlightLight = spotlight.GetComponent<Light>();
        if (spotlightLight == null)
        {
            Debug.LogError("スポットライトにLightコンポーネントがありません！");
            enabled = false;
            return;
        }

        // 初期位置をカメラの位置に設定
        targetPosition = spotlight.position;

        // 次のランダムオフタイミングを設定
        ScheduleNextRandomOff();
    }

    void LateUpdate()
    {
        // コントローラーのYボタンでオンオフ切り替え
        if (Input.GetButtonDown("Fire4"))
        {
            isSpotlightOn = !isSpotlightOn;
            spotlightLight.enabled = isSpotlightOn;
        }

        // ランダムなタイミングでスポットライトを一瞬オフにする
        if (Time.time >= nextRandomOffTime)
        {
            StartCoroutine(RandomOff());
            ScheduleNextRandomOff();
        }

        // スポットライトがオンの場合に追従
        if (isSpotlightOn)
        {
            targetPosition = transform.position;

            spotlight.position = Vector3.Lerp(spotlight.position, targetPosition, lagSpeed * Time.deltaTime);
            spotlight.rotation = Quaternion.Lerp(spotlight.rotation, transform.rotation, lagSpeed * Time.deltaTime);
        }
    }

    private void ScheduleNextRandomOff()
    {
        // 次のランダムオフタイミングを設定
        nextRandomOffTime = Time.time + Random.Range(randomOffInterval * 0.5f, randomOffInterval * 1.5f);
    }

    private System.Collections.IEnumerator RandomOff()
    {
        // スポットライトを一瞬オフにする
        spotlightLight.enabled = false;
        yield return new WaitForSeconds(randomOffDuration);
        spotlightLight.enabled = isSpotlightOn; // 現在の状態に戻す
    }
}

