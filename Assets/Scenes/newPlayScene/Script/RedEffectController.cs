using UnityEngine;
using UnityEngine.UI;

public class EdgeRedEffect : MonoBehaviour
{
    public string targetTag = "Enemy"; // �^�[�Q�b�g�̃^�O�i��: Enemy�j
    public float maxEffectDistance = 5f; // �ő�G�t�F�N�g�͈�
    public Image topEdgeImage;  // �㕔�̃G�b�W�ɓK�p����Image
    public Image bottomEdgeImage;  // �����̃G�b�W�ɓK�p����Image
    public Image leftEdgeImage;  // �����̃G�b�W�ɓK�p����Image
    public Image rightEdgeImage;  // �E���̃G�b�W�ɓK�p����Image
    public Color maxEffectColor = new Color(1f, 0f, 0f, 0.5f); // �ő�G�t�F�N�g�̐F�i�ԁj
    public Color minEffectColor = new Color(1f, 0f, 0f, 0f); // �ŏ��G�t�F�N�g�i�����j

    private void Update()
    {
        // "Enemy"�^�O�����I�u�W�F�N�g���擾
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            // �v���C���[�ƓG�̋������v�Z
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // �ł��߂��G��������
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        // �����ɉ����ăG�t�F�N�g�̋������v�Z
        float effectStrength = Mathf.Clamp01(1 - (closestDistance / maxEffectDistance));

        // �G�t�F�N�g�̐F���X�V�i�S�ẴG�b�WImage�ɓK�p�j
        Color currentEffectColor = Color.Lerp(minEffectColor, maxEffectColor, effectStrength);
        topEdgeImage.color = currentEffectColor;
        bottomEdgeImage.color = currentEffectColor;
        leftEdgeImage.color = currentEffectColor;
        rightEdgeImage.color = currentEffectColor;
    }
}
