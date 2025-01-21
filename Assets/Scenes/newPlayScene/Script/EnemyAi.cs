using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public float detectionRange = 5f;  // �v���C���[�����o����͈�
    public float chaseRange = 10f;     // �v���C���[��ǐՂ���͈�
    public float loseSightTime = 2f;   // �v���C���[���͈͂ɓ���Ȃ���Ό������܂ł̎���
    public float speed = 5f;           // �G�l�~�[�̈ړ����x

    private Transform player;          // �v���C���[��Transform
    private NavMeshAgent navAgent;     // �G�l�~�[��NavMeshAgent
    private bool isChasing = false;    // �G�l�~�[���v���C���[��ǐՂ��Ă��邩�ǂ���
    private float timeSinceLastSeen = 0f;  // �v���C���[���������Ă���o�߂�������

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;  // Player�^�O�����I�u�W�F�N�g��T��
        if (player == null)
        {
            Debug.LogWarning("Player�^�O�����I�u�W�F�N�g��������܂���I");
            return;  // Player��������Ȃ���ΒǐՂ��Ȃ�
        }
        navAgent = GetComponent<NavMeshAgent>();  // NavMeshAgent�R���|�[�l���g���擾
        navAgent.speed = speed;  // �G�l�~�[�̈ړ����x��ݒ�
    }

    void Update()
    {
        if (player == null)
            return;  // Player�����݂��Ȃ��ꍇ�A�����𒆒f

        // �v���C���[��"Player"�^�O�������Ă��邩�`�F�b�N
        if (!player.CompareTag("Player"))
            return;  // "Player"�^�O���Ȃ��ꍇ�A�ǐՂ��Ȃ�

        // �v���C���[�Ƃ̋������v�Z
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �v���C���[���ǐՔ͈͂ɓ������ꍇ
        if (distanceToPlayer <= chaseRange)
        {
            // �v���C���[���͈͓��ɂ���ꍇ�A�ǐՂ��J�n
            if (distanceToPlayer <= detectionRange)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    timeSinceLastSeen = 0f;  // �����������Ԃ����Z�b�g
                }
                navAgent.SetDestination(player.position);  // �v���C���[�̈ʒu�Ɍ������Ĉړ�
            }
        }
        else
        {
            // �ǐՔ͈͊O�̏ꍇ�A�ʏ퓮��ɖ߂�
            if (isChasing)
            {
                isChasing = false;
                navAgent.ResetPath();  // �ǐՂ��~
            }
        }

        // �v���C���[���͈͂���o�Ă���2�b�o�߂����猩�������Ƃ݂Ȃ��Ēʏ퓮��ɖ߂�
        if (isChasing)
        {
            timeSinceLastSeen += Time.deltaTime;
            if (timeSinceLastSeen >= loseSightTime)
            {
                isChasing = false;
                navAgent.ResetPath();  // �������Ēʏ퓮��ɖ߂�
            }
        }
    }
}
