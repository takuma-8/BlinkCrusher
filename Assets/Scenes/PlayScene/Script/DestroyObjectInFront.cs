using UnityEngine;

public class DestroyObjectInFront : MonoBehaviour
{
    public float detectionRadius = 1.0f;  // ���o���锼�a
    public string targetTag = "Target";   // �j�󂵂����I�u�W�F�N�g�̃^�O
    private ObjectSpawner objectSpawner;  // ObjectSpawner�̃C���X�^���X���Q��

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
        // Player�̐���1���[�g����̈ʒu���v�Z
        Vector3 frontPosition = transform.position + transform.forward * detectionRadius;

        // OverlapSphere�Ŏw��͈͓��̃R���C�_�[���擾
        Collider[] colliders = Physics.OverlapSphere(frontPosition, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(targetTag))  // �^�O����v����I�u�W�F�N�g��j��
            {
                // ObjectSpawner�ɒʒm���ă��X�g����폜
                if (objectSpawner != null)
                {
                    objectSpawner.RemoveSpawnedObject(collider.gameObject);
                }

                // �I�u�W�F�N�g��j��
                Destroy(collider.gameObject);
                break;  // 1�����j�󂷂�ꍇ�̓��[�v���I��
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
}
