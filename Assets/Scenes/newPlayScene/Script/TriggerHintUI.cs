using UnityEngine;
using UnityEngine.UI;

public class TriggerHintUI : MonoBehaviour
{
    public Text hintText; // ヒントメッセージ用のUIテキスト
    private PlayerTeleport playerTeleport; // 親オブジェクトにあるPlayerTeleportへの参照

    private bool previousStateIsEmperor;
    private bool previousStateInRange;

    private void Start()
    {
        // 親オブジェクトからPlayerTeleportを取得
        playerTeleport = GetComponentInParent<PlayerTeleport>();

        if (hintText == null)
        {
            Debug.LogError("HintTextが設定されていません！InspectorでUIテキストを設定してください。");
            enabled = false;
            return;
        }

        if (playerTeleport == null)
        {
            Debug.LogError("PlayerTeleportが親オブジェクトに見つかりません！スクリプトを確認してください。");
            enabled = false;
            return;
        }

        hintText.gameObject.SetActive(false); // 初期状態では非表示
    }

    private void Update()
    {
        if (playerTeleport == null || playerTeleport.targetObjects == null || playerTeleport.targetObjects.Count == 0)
        {
            return;
        }

        bool isInRange = false;
        bool isEmperor = playerTeleport.IsEmperor();

        // トリガー範囲をチェック
        foreach (Transform triggerObject in playerTeleport.targetObjects)
        {
            if (Vector3.Distance(playerTeleport.transform.position, triggerObject.position) <= playerTeleport.triggerRange)
            {
                isInRange = true;
                break;
            }
        }

        // 状態が変化した場合のみ UI を更新
        if (isEmperor != previousStateIsEmperor || isInRange != previousStateInRange)
        {
            if (isEmperor)
            {
                hintText.text = "Bボタンで出る"; // 隠れている状態のメッセージ
                hintText.gameObject.SetActive(true);
            }
            else if (isInRange)
            {
                hintText.text = "Bボタンで隠れる"; // 通常時のメッセージ
                hintText.gameObject.SetActive(true);
            }
            else
            {
                hintText.gameObject.SetActive(false); // 非表示
            }

            previousStateIsEmperor = isEmperor;
            previousStateInRange = isInRange;
        }
    }
}