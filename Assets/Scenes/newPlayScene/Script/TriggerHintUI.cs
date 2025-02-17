using UnityEngine;
using UnityEngine.UI;

public class TriggerHintUI : MonoBehaviour
{
    public Image showHintSprite; // 「Bボタンで出る」の画像
    public Image hideHintSprite; // 「Bボタンで隠れる」の画像

    private PlayerTeleport playerTeleport; // 親オブジェクトにあるPlayerTeleportへの参照
    private bool previousStateIsEmperor;
    private bool previousStateInRange;

    private void Start()
    {
        // 親オブジェクトから PlayerTeleport を取得
        playerTeleport = GetComponentInParent<PlayerTeleport>();

        if (playerTeleport == null)
        {
            Debug.LogError("PlayerTeleportが親オブジェクトに見つかりません！スクリプトを確認してください。");
            enabled = false;
            return;
        }

        // 初期状態で UI を非表示にする
        showHintSprite.enabled = false;
        hideHintSprite.enabled = false;
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
                // エンペラー状態なら「Bボタンで出る」のみ表示
                showHintSprite.enabled = true;
                hideHintSprite.enabled = false;
            }
            else if (isInRange)
            {
                // 範囲内なら「Bボタンで隠れる」のみ表示
                showHintSprite.enabled = false;
                hideHintSprite.enabled = true;
            }
            else
            {
                // どちらにも該当しなければ非表示
                showHintSprite.enabled = false;
                hideHintSprite.enabled = false;
            }

            // 状態を更新
            previousStateIsEmperor = isEmperor;
            previousStateInRange = isInRange;
        }
    }
}
