using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Secret_Enemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    // 追いかける対象のタグ
    [SerializeField]
    private string _playerTag = "Player";

    private Transform _player;

    void Awake()
    {
        // タグからPlayerオブジェクトを取得
        GameObject playerObject = GameObject.FindWithTag(_playerTag);
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
        else
        {
            Debug.LogError($"タグ '{_playerTag}' を持つオブジェクトが見つかりません！");
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
