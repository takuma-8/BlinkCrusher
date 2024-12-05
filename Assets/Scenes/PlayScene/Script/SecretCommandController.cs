using UnityEngine;

public class SecretCommand : MonoBehaviour
{
    // 隠しコマンドの順番
    private readonly string[] secretCommand = {
        "TriggerLeft", "TriggerRight",
        "X", "Y",
        "TriggerLeft", "TriggerRight",
        "X", "Y",
        "TriggerLeft", "TriggerRight",
        "X", "X"
    };

    private int inputIndex = 0; // 現在の入力位置を追跡
    private float inputCooldown = 0.2f; // 入力クールダウン（次の入力が許される間隔）
    private float lastInputTime = 0f; // 最後の入力時間を記録
    private bool isSecretEnemySummoned = false; // 一度だけ召喚するフラグ

    // Secret_EnemyのPrefabをアサイン
    public GameObject secretEnemyPrefab;

    void Update()
    {
        if (Input.GetAxis("TriggerLeft") > 0.8f) Debug.Log("TriggerLeft pressed");
        if (Input.GetAxis("TriggerRight") > 0.8f) Debug.Log("TriggerRight pressed");
        if (Input.GetButtonDown("X")) Debug.Log("X pressed");
        if (Input.GetButtonDown("Y")) Debug.Log("Y pressed");
        if (Time.time - lastInputTime < inputCooldown)
            return; // クールダウン中は入力を無視

        // コマンド入力チェック
        if (CheckInput(secretCommand[inputIndex]))
        {
            inputIndex++;
            lastInputTime = Time.time; // 入力を記録

            // 全てのコマンドが成功した場合
            if (inputIndex >= secretCommand.Length && !isSecretEnemySummoned)
            {
                Debug.Log("隠しコマンドが成功しました！");
                ActivateSecretFeature(); // 隠し機能を発動
                inputIndex = 0; // リセット
            }
        }
        else if (AnyInputDetected()) // 他の入力があればリセット
        {
            inputIndex = 0;
        }
    }

    // 入力チェック
    private bool CheckInput(string command)
    {
        switch (command)
        {
            case "TriggerLeft":
                return Input.GetAxis("TriggerLeft") > 0.8f; // 左トリガー入力
            case "TriggerRight":
                return Input.GetAxis("TriggerRight") > 0.8f; // 右トリガー入力
            case "Y":
                return Input.GetButtonDown("Y"); // Yボタン入力
            case "X":
                return Input.GetButtonDown("X"); // Xボタン入力
            default:
                return false;
        }
    }

    // 任意の入力が発生したかを確認
    private bool AnyInputDetected()
    {
        return Input.GetAxis("TriggerLeft") > 0.1f ||
               Input.GetAxis("TriggerRight") > 0.1f ||
               Input.GetButtonDown("Y") ||
               Input.GetButtonDown("X");
    }

    // 隠しコマンドの発動時に実行される機能
    private void ActivateSecretFeature()
    {
        if (!isSecretEnemySummoned && secretEnemyPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(43.63f, 1.03f, -3.48f);
            Instantiate(secretEnemyPrefab, spawnPosition, Quaternion.identity); // Secret_Enemyを召喚
            Debug.Log("Secret_Enemyが召喚されました！");
            isSecretEnemySummoned = true; // フラグを立てる
        }
    }
}
