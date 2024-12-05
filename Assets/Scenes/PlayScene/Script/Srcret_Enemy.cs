using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Secret_Enemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    // �ǂ�������Ώۂ̃^�O
    [SerializeField]
    private string _playerTag = "Player";

    private Transform _player;

    void Awake()
    {
        // �^�O����Player�I�u�W�F�N�g���擾
        GameObject playerObject = GameObject.FindWithTag(_playerTag);
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
        else
        {
            Debug.LogError($"�^�O '{_playerTag}' �����I�u�W�F�N�g��������܂���I");
        }
    }

    void Update()
    {
        if (_player != null)
        {
            _navMeshAgent.SetDestination(_player.position);
        }
    }
}
