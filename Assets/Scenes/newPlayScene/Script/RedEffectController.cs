using UnityEngine;
using UnityEngine.UI;

public class EdgeRedEffect : MonoBehaviour
{
    public string targetTag = "Enemy"; // ターゲットのタグ（例: Enemy）
    public float maxEffectDistance = 5f; // 最大エフェクト範囲
    public Image topEdgeImage;  // 上部のエッジに適用するImage
    public Image bottomEdgeImage;  // 下部のエッジに適用するImage
    public Image leftEdgeImage;  // 左側のエッジに適用するImage
    public Image rightEdgeImage;  // 右側のエッジに適用するImage
    public Color maxEffectColor = new Color(1f, 0f, 0f, 0.5f); // 最大エフェクトの色（赤）
    public Color minEffectColor = new Color(1f, 0f, 0f, 0f); // 最小エフェクト（透明）

    private void Update()
    {
        // "Enemy"タグを持つオブジェクトを取得
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            // プレイヤーと敵の距離を計算
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // 最も近い敵を見つける
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        // 距離に応じてエフェクトの強さを計算
        float effectStrength = Mathf.Clamp01(1 - (closestDistance / maxEffectDistance));

        // エフェクトの色を更新（全てのエッジImageに適用）
        Color currentEffectColor = Color.Lerp(minEffectColor, maxEffectColor, effectStrength);
        topEdgeImage.color = currentEffectColor;
        bottomEdgeImage.color = currentEffectColor;
        leftEdgeImage.color = currentEffectColor;
        rightEdgeImage.color = currentEffectColor;
    }
}
