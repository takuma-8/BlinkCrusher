using UnityEngine;
using UnityEngine.AI;

public class MoveToRandomTarget : MonoBehaviour
{
    public float minRange = 5.0f; // �Œ�͈�
    public float maxRange = 10.0f; // �ō��͈�
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination(); // �����_���ȖړI�n��ݒ�
    }

    void Update()
    {
        // �I�u�W�F�N�g���ړ����ɁA�ړ������ɐ��ʂ���������
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
        float randomDistance = Random.Range(minRange, maxRange); // �Œ�ƍō��͈͂̊ԂŃ����_���ȋ����𐶐�
        Vector3 randomDirection = Random.insideUnitSphere.normalized * randomDistance; // �����_���ȕ����Ƌ���
        randomDirection += transform.position; // ���݈ʒu����ɂ���

        NavMeshHit hit;
        // NavMesh��Ń����_���|�C���g��������
        if (NavMesh.SamplePosition(randomDirection, out hit, maxRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
