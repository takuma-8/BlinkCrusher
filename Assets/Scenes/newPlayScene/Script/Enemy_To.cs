using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_To: MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animation;

    void Start()
    {
        animation = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Enter�L�[�������ꂽ���ǂ������`�F�b�N���܂�
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Animator����"boolwalk"�p�����[�^��true�ɐݒ肵�A���s�A�j���[�V�������Đ�����
            animation.SetBool("blRot" +
                "", true);
        }
    }
}