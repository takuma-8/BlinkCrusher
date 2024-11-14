using UnityEngine;

public class DestroyObjectInFront : MonoBehaviour
{
    public float detectionRadius = 1.0f;  // ���o���锼�a
    public string targetTag1 = "Target1"; // �^�[�Q�b�g1�̃^�O
    public string targetTag2 = "Target2"; // �^�[�Q�b�g2�̃^�O
    private ObjectSpawner objectSpawner;   // ObjectSpawner�̎Q��
    public static int score = 0;          // ���L�X�R�A�ϐ�

    void Start()
    {
        // ObjectSpawner�̃C���X�^���X���擾
        objectSpawner = FindObjectOfType<ObjectSpawner>();
    }

    void Update()
    {
        DetectAndDestroy();
    }

    void DetectAndDestroy()
    {
        // �v���C���[�̐���1���[�g����̈ʒu���v�Z
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;

        // OverlapSphere�Ŏw��͈͓��̃R���C�_�[���擾
        Collider[] colliders = Physics.OverlapSphere(frontPosition, detectionRadius);

        foreach (Collider collider in colliders)
        {
            // �^�O��Target1�܂���Target2�̏ꍇ�ɔj��
            if (collider.CompareTag(targetTag1) || collider.CompareTag(targetTag2))
            {
                // ObjectSpawner�ɒʒm���ă��X�g����폜
                if (objectSpawner != null)
                {
                    objectSpawner.RemoveSpawnedObject(collider.gameObject);
                }

                // �^�O�ɉ����ăX�R�A�����Z
                if (collider.CompareTag(targetTag1))
                {
                    score += 100;  // Target1�ɂ�10�|�C���g
                }
                else if (collider.CompareTag(targetTag2))
                {
                    score += 500;   // Target2�ɂ�5�|�C���g
                }

                // �I�u�W�F�N�g��j��
                Destroy(collider.gameObject);
                break;  // 1�����j�󂷂�
            }
        }
    }

    // Gizmos�Ō��o�͈͂�\��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;
        Gizmos.DrawWireSphere(frontPosition, detectionRadius);
    }

    // ���݂̃X�R�A���擾���邽�߂̐ÓI���\�b�h
    public static int GetScore()
    {
        return score;
    }
}
