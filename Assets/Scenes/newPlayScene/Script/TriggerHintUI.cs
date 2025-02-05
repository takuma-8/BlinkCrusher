using UnityEngine;
using UnityEngine.UI;

public class TriggerHintUI : MonoBehaviour
{
    public Image hintImage; // ヒント用のUI画像
    public Sprite showHintSprite; // 「Bボタンで出る」の画像
    public Sprite hideHintSprite; // 「Bボタンで隠れる」の画像

    private PlayerTeleport playerTeleport; // 親オブジェクトにあるPlayerTeleportへの参照
    private bool previousStateIsEmperor;
    private bool previousStateInRange;

    private void Start()
    {
        // 親オブジェクトからPlayerTeleportを取得
        playerTeleport = GetComponentInParent<PlayerTeleport>();

        if (hintImage == null)
        {
            Debug.LogError("HintImageが設定されていません！InspectorでUI Imageを設定してください。");
            enabled = false;
            return;
        }

        if (playerTeleport == null)
        {
            Debug.LogError("PlayerTeleportが親オブジェクトに見つかりません！スクリプトを確認してください。");
            enabled = false;
            return;
        }

        hintImage.enabled = false; // 初期状態では非表示
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
                hintImage.sprite = showHintSprite; // 「Bボタンで出る」の画像に変更
                hintImage.enabled = true; // 画像を表示
            }
            else if (isInRange)
            {
                hintImage.sprite = hideHintSprite; // 「Bボタンで隠れる」の画像に変更
                hintImage.enabled = true; // 画像を表示
            }
            else
            {
                hintImage.enabled = false; // 画像を非表示
            }

            previousStateIsEmperor = isEmperor;
            previousStateInRange = isInRange;
        }
    }
}
