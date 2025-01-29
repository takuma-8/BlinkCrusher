using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        // �X�s�[�h��0�Ȃ�A�j���[�V�������~�߂�
        if (speed < 0.1f)
        {
            animator.speed = 0;
        }
        else
        {
            animator.speed = 1;
        }
    }

}
