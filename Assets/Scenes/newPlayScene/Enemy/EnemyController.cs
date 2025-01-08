using System.Collections;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent ���g�p����ꍇ

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Enemy1, Enemy2 };
    public EnemyType enemyType;

    public GameObject visionObject;       // ���E�I�u�W�F�N�g
    private bool isStunned = false;
    private NavMeshAgent agent;          // NavMeshAgent ���g�p����ꍇ
    private Renderer enemyRenderer;      // �G�̃}�e���A����؂�ւ��邽�߂� Renderer

    private Vector3 wanderTarget;
    private bool isWandering = false;


    private void Start()
    {
        // NavMeshAgent ���擾
        agent = GetComponent<NavMeshAgent>();

        // Renderer ���擾
        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer == null)
        {
            Debug.LogError("Enemy does not have a Renderer component!");
        }
    }

   

 
    public void WanderForSeconds(Vector3 origin, float duration)
    {
        if (agent == null || isStunned) return;

        StartCoroutine(WanderCoroutine(origin, duration));
    }

    private IEnumerator WanderCoroutine(Vector3 origin, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // �����_���ȃ|�C���g�𐶐�
            Vector3 randomPoint = origin + new Vector3(
                Random.Range(-2.5f, 2.5f),
                0,
                Random.Range(-2.5f, 2.5f)
            );

            // NavMesh ��̗L���Ȉʒu�ɕϊ�
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            // ���̈ړ��܂ŏ����҂�
            yield return new WaitForSeconds(1f);

            elapsed += 1f;
        }

        // ������I����A��~������i�K�v�Ȃ璲���j
        agent.SetDestination(transform.position);
    }
    //public void MoveToPositionAndStop(Vector3 targetPosition)
    //{
    //    if (agent == null || isStunned) return;

    //    // NavMeshAgent�Ŏw��ʒu�Ɉړ�
    //    agent.SetDestination(targetPosition);

    //    // ��~���`�F�b�N����R���[�`�����J�n
    //    StartCoroutine(StopAtTargetCoroutine(targetPosition));
    //}

    public void MoveToPositionAndWander(Vector3 targetPosition, float wanderDuration = 5f, float wanderRadius = 2f)
    {
        if (agent == null || isStunned) return;

        ////�F��ύX
        //if (enemyRenderer != null)
        //{
        //    enemyRenderer.material.color = Color.red;
        //}

        // NavMeshAgent�Ŏw��ʒu�Ɉړ�
        agent.SetDestination(targetPosition);

        StartCoroutine(WanderAfterReachingTarget(targetPosition, wanderDuration, wanderRadius));
    }

    private IEnumerator WanderAfterReachingTarget(Vector3 targetPosition, float wanderDuration, float wanderRadius)
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            Debug.Log($"{gameObject.name} Remaining Distance: {agent.remainingDistance}, Stopping Distance: {agent.stoppingDistance}");
            yield return null;
        }


        Debug.Log($"{gameObject.name} has reached position {targetPosition}");

        // ���낤����J�n
        float elapsedTime = 0f;
        while (elapsedTime < wanderDuration)
        {
            Vector3 randomPoint = targetPosition + new Vector3(
                Random.Range(-wanderRadius, wanderRadius),
                0,
                Random.Range(-wanderRadius, wanderRadius)
            );

            // NavMesh ��̗L���Ȉʒu���m�F
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log($"{gameObject.name} is wandering to {hit.position}");
            }
            else
            {
                Debug.LogWarning($"Failed to find valid NavMesh position near {randomPoint}");
            }


            // ���̈ړ��܂őҋ@
            float wanderInterval = Random.Range(1f, 2f);
            yield return new WaitForSeconds(wanderInterval);

            elapsedTime += wanderInterval;

            // �i�s�󋵂̃f�o�b�O
            Debug.Log($"{gameObject.name} has been wandering for {elapsedTime} seconds.");
        }

        Debug.Log($"{gameObject.name} finished wandering near {targetPosition}");
    }



    private IEnumerator StopAtTargetCoroutine(Vector3 targetPosition)
    {
        // ���B����܂ő҂�
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // ���B��ANavMeshAgent���~
        agent.isStopped = true;

        // �f�o�b�O�p���O
        Debug.Log($"{gameObject.name} has stopped at position {targetPosition}");
    }
}
