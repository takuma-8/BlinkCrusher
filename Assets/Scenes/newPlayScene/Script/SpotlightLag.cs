using UnityEngine;

public class SpotlightLag : MonoBehaviour
{
    public Transform spotlight; // �X�|�b�g���C�g��Transform
    public float lagSpeed = 2f; // �x���̑��x
    public float randomOffInterval = 0.1f; // �����_���ŃI�t�ɂȂ�܂ł̕��ϊԊu�i�b�j
    public float randomOffDuration = 0.1f; // �����_���ŃI�t�ɂȂ鎞�ԁi�b�j

    private Vector3 targetPosition;
    private Light spotlightLight; // �X�|�b�g���C�g��Light�R���|�[�l���g
    private bool isSpotlightOn = true; // �X�|�b�g���C�g�̃I���I�t���
    private float nextRandomOffTime; // ���Ƀ����_���ŃI�t�ɂȂ鎞��

    void Start()
    {
        if (spotlight == null)
        {
            Debug.LogError("�X�|�b�g���C�g���ݒ肳��Ă��܂���I");
            enabled = false;
            return;
        }

        // Light�R���|�[�l���g���擾
        spotlightLight = spotlight.GetComponent<Light>();
        if (spotlightLight == null)
        {
            Debug.LogError("�X�|�b�g���C�g��Light�R���|�[�l���g������܂���I");
            enabled = false;
            return;
        }

        // �����ʒu���J�����̈ʒu�ɐݒ�
        targetPosition = spotlight.position;

        // ���̃����_���I�t�^�C�~���O��ݒ�
        ScheduleNextRandomOff();
    }

    void LateUpdate()
    {
        // �R���g���[���[��Y�{�^���ŃI���I�t�؂�ւ�
        if (Input.GetButtonDown("Fire4"))
        {
            isSpotlightOn = !isSpotlightOn;
            spotlightLight.enabled = isSpotlightOn;
        }

        // �����_���ȃ^�C�~���O�ŃX�|�b�g���C�g����u�I�t�ɂ���
        if (Time.time >= nextRandomOffTime)
        {
            StartCoroutine(RandomOff());
            ScheduleNextRandomOff();
        }

        // �X�|�b�g���C�g���I���̏ꍇ�ɒǏ]
        if (isSpotlightOn)
        {
            targetPosition = transform.position;

            spotlight.position = Vector3.Lerp(spotlight.position, targetPosition, lagSpeed * Time.deltaTime);
            spotlight.rotation = Quaternion.Lerp(spotlight.rotation, transform.rotation, lagSpeed * Time.deltaTime);
        }
    }

    private void ScheduleNextRandomOff()
    {
        // ���̃����_���I�t�^�C�~���O��ݒ�
        nextRandomOffTime = Time.time + Random.Range(randomOffInterval * 0.5f, randomOffInterval * 1.5f);
    }

    private System.Collections.IEnumerator RandomOff()
    {
        // �X�|�b�g���C�g����u�I�t�ɂ���
        spotlightLight.enabled = false;
        yield return new WaitForSeconds(randomOffDuration);
        spotlightLight.enabled = isSpotlightOn; // ���݂̏�Ԃɖ߂�
    }
}

