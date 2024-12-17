using System.Collections;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent ���g�p����ꍇ

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Enemy1, Enemy2 };
    public EnemyType enemyType;

    public GameObject visionObject;       // ���E�I�u�W�F�N�g
    public float stunnedDuration = 10f;  // �C�⎞��
    public Material blinkMaterial;       // �_�Ŏ��̃}�e���A���i�K�{�j
    public Material normalMaterial;      // �ʏ펞�̃}�e���A���i�K�{�j

    private bool isStunned = false;
    private NavMeshAgent agent;          // NavMeshAgent ���g�p����ꍇ
    private Renderer enemyRenderer;      // �G�̃}�e���A����؂�ւ��邽�߂� Renderer

    // Enemy2�p�̃p�����[�^
    public float lifetime = 10.0f; // �\������鎞�ԁi�b�j
    private bool isTemporary; // Enemy2���ǂ����𔻒�
    private bool isDisappearing = false; // �����铮�쒆������

    [SerializeField]
    private Vector3 disappearPosition = new Vector3(44.0f, 1.0f, 20.0f); // Enemy2��������ʒu

    private void Start()
    {
        disappearPosition = new Vector3(44.0f, 1.0f, 20.0f);

        // NavMeshAgent ���擾
        agent = GetComponent<NavMeshAgent>();

        // Renderer ���擾
        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer == null)
        {
            Debug.LogError("Enemy does not have a Renderer component!");
        }

        if(enemyType == EnemyType.Enemy1) 
        {
            InitializeEnemy1();
        }
        else if(enemyType == EnemyType.Enemy2)
        {
            InitializeEnemy2();
        }
    }

    private void InitializeEnemy1()
    {
        Debug.Log("Enemy1 initialized.");
        // Enemy1��p�̏����������i�K�v�Ȃ�ǉ��j
    }

    private void InitializeEnemy2()
    {
        Debug.Log("Enemy2 initialized with lifetime.");
        Debug.Log($"Disappear position is set to: {disappearPosition}"); // �f�o�b�O�p���O
        isTemporary = true;
        StartCoroutine(DeactivateAfterLifetime(lifetime));
    }

    private IEnumerator DeactivateAfterLifetime(float time)
    {
        agent.stoppingDistance = 1.0f;
        yield return new WaitForSeconds(time);

        if (isDisappearing) yield break;

        isDisappearing = true;

        if (agent != null)
        {
            // �����蔻��𖳌���
            Collider collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            // NavMeshAgent ���g�p���Ĉړ�
            agent.isStopped = false;
            agent.SetDestination(disappearPosition);

            // �ړ����̃f�o�b�O
            Debug.Log($"{gameObject.name} is moving to disappear position: {disappearPosition}");

            // ����̈ʒu�ɓ��B����܂őҋ@
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                Debug.Log($"Remaining Distance: {agent.remainingDistance}, Path Status: {agent.pathStatus}");
                yield return null;
            }
        }
        else
        {
            transform.position = disappearPosition;
            Debug.Log($"Enemy moved directly to: {disappearPosition}");
        }

        // ������ɃI�u�W�F�N�g���폜
        Debug.Log($"{gameObject.name} has disappeared at position {disappearPosition}.");
        Destroy(gameObject);
    }

    public void Stun()
    {
        if (isStunned) return; // ���ɋC�⒆�Ȃ珈�����Ȃ�

        isStunned = true;

        // �f�o�b�O���b�Z�[�W
        Debug.Log($"{gameObject.name} is stunned for {stunnedDuration} seconds!");

        // ���E�I�u�W�F�N�g���A�N�e�B�u��
        if (visionObject != null)
        {
            visionObject.SetActive(false);
        }

        // NavMeshAgent ���~����i�����g�p���Ă���΁j
        if (agent != null)
        {
            agent.isStopped = true;
        }

        // �_�ŏ������J�n
        StartCoroutine(BlinkEffect());

        // �C���Ԃ��Č�
        StartCoroutine(StunnedCoroutine());
    }

    private IEnumerator StunnedCoroutine()
    {
        // �C�⒆�͓��삵�Ȃ�
        yield return new WaitForSeconds(stunnedDuration);

        // �C�����
        isStunned = false;

        // ���E�I�u�W�F�N�g���ēx�A�N�e�B�u��
        if (visionObject != null)
        {
            visionObject.SetActive(true);
        }

        // NavMeshAgent ���ĊJ����i�����g�p���Ă���΁j
        if (agent != null)
        {
            agent.isStopped = false;
        }

        // �ʏ��Ԃ̃}�e���A���ɖ߂�
        if (enemyRenderer != null && normalMaterial != null)
        {
            enemyRenderer.material = normalMaterial;  // �������}�e���A����ݒ�
        }

        // �f�o�b�O���b�Z�[�W
        Debug.Log($"{gameObject.name} has recovered from being stunned.");
    }

    private IEnumerator BlinkEffect()
    {
        if (enemyRenderer == null || blinkMaterial == null || normalMaterial == null)
        {
            Debug.LogError("BlinkEffect: Renderer or materials are not set properly.");
            yield break;
        }

        float elapsedTime = 0f;
        bool isBlinking = false;

        while (isStunned)
        {
            enemyRenderer.material = isBlinking ? normalMaterial : blinkMaterial;
            isBlinking = !isBlinking;

            // �_�ŊԊu��ݒ�
            yield return new WaitForSeconds(0.3f);

            elapsedTime += 0.3f;
        }

        // �C�₪�I�������ʏ�̃}�e���A���ɖ߂�
        enemyRenderer.material = normalMaterial;
    }
}
