using UnityEngine;
using UnityEngine.AI;

public class MoveToRandomTarget : MonoBehaviour
{
    public float range = 10.0f; // �����_���͈͂̔��a
    public float minDistance = 5.0f; // �ŏ�����
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination(); // �����_���ȖړI�n��ݒ�
    }

    void Update()
    {
        // �I�u�W�F�N�g���^�[�Q�b�g�Ɍ������Ă���ԁA�ړ������ɐ��ʂ���������
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }

        // �G�[�W�F���g���ڕW�n�_�ɓ��B���Ă��邩�m�F���A�Đݒ�
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetRandomDestination(); // �ڕW�ɓ��B��A�ēx�����_���ȏꏊ��ݒ�
        }
    }

    // �����_���ȏꏊ�ɖړI�n��ݒ肷�郁�\�b�h
    void SetRandomDestination()
    {
        Vector3 randomDirection;
        NavMeshHit hit;
        bool validPosition = false;

        // �L���ȖړI�n��������܂ŌJ��Ԃ�
        do
        {
            randomDirection = Random.insideUnitSphere * range; // �����_���ȕ������擾
            randomDirection += transform.position; // ���݈ʒu����ɂ���

            // NavMesh��Ń����_���|�C���g�������A�L���ȏꍇ��true���Ԃ�
            if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
            {
                // �ŏ������𖞂����Ă��邩�m�F
                if (Vector3.Distance(transform.position, hit.position) >= minDistance)
                {
                    agent.SetDestination(hit.position);
                    validPosition = true;
                }
            }
        } while (!validPosition); // �L���ȖړI�n��������܂Ń��[�v
    }
}
