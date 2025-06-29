using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �G�̍U���p�^�[���i�\�����U���j���Ǘ�����V�X�e���B
/// 3x3�̃}�X�ɑ΂��ă����_���ɍU�����d�|���܂��B
/// </summary>
public class EnemyAttackSystem : MonoBehaviour
{
    [Header("�� �֘A�I�u�W�F�N�g�̐ݒ�")]
    [Tooltip("�U���\����\������9��UI�I�u�W�F�N�g")]
    public List<GameObject> attentionMarks;
    [Tooltip("�U�����ɐ�������G�t�F�N�g�̃v���n�u")]
    public GameObject attackEffectPrefab;
    [Tooltip("�U���ΏۂƂȂ�v���C���[1")]
    public PlayerController playerController1;
    [Tooltip("�U���ΏۂƂȂ�v���C���[2")]
    public PlayerController playerController2;

    [Header("�� �U���p�����[�^")]
    [Tooltip("���̍U���\�����n�܂�܂ł̊Ԋu�i�b�j")]
    public float interval = 10f;
    [Tooltip("�U���\�����\������Ă���U������������܂ł̎��ԁi�b�j")]
    public float displayDuration = 2f;
    [Tooltip("��x�ɍU������}�X�̐�")]
    public int glowCount = 3;
    [Tooltip("�U�����q�b�g�����ۂ̃X�^�����ԁi�b�j")]
    public float stunDuration = 1.5f;

    /// <summary>
    /// �R���|�[�l���g���������̏���
    /// </summary>
    void Start()
    {
        // Inspector�ł̐ݒ肪������������
        if (attentionMarks == null || attentionMarks.Count != 9 || attackEffectPrefab == null || playerController1 == null || playerController2 == null)
        {
            Debug.LogError("EnemyAttackSystem�̃C���X�y�N�^�[�ݒ肪�s�����Ă��܂��B�I�u�W�F�N�g���A�^�b�`���Ă��������B");
            enabled = false; // �G���[���̓R���|�[�l���g�𖳌���
            return;
        }

        // �Q�[���J�n���ɑS�Ă̗\�����\���ɂ���
        foreach (GameObject mark in attentionMarks)
        {
            if (mark != null) mark.SetActive(false);
        }

        // �U���̃��C�����[�v���J�n
        StartCoroutine(AttackLoop());
    }

    /// <summary>
    /// �u�\�����ҋ@���U���v�̃T�C�N���𖳌��ɌJ��Ԃ��R���[�`��
    /// </summary>
    private IEnumerator AttackLoop()
    {
        while (true)
        {
            // 1. �U���}�X�̑I��
            // 0����8�̃C���f�b�N�X�������_���ɕ��בւ��A�擪����w����������I������
            List<int> chosenIndices = Enumerable.Range(0, 9).OrderBy(x => Random.value).Take(glowCount).ToList();

            // 2. �U���̗\��
            // �I�񂾃}�X�̗\�����A�N�e�B�u�ɂ���
            foreach (int index in chosenIndices)
            {
                attentionMarks[index].SetActive(true);
            }

            // 3. �v���C���[�̉������
            // �\�����\������Ă���ԁA�ҋ@����
            yield return new WaitForSeconds(displayDuration);

            // 4. �U���̎��s�Ɠ����蔻��
            foreach (int index in chosenIndices)
            {
                // �U�������Ɠ����ɗ\�����\���ɂ���
                attentionMarks[index].SetActive(false);

                // �U���G�t�F�N�g��\���Ɠ����ʒu�ɐ���
                Instantiate(attackEffectPrefab, attentionMarks[index].transform.position, Quaternion.identity);

                // �U���}�X�̃O���b�h���W���v�Z (��: �C���f�b�N�X5 -> X:2, Y:1)
                int attackX = index % 3;
                int attackY = index / 3;

                // --- �v���C���[1�̓����蔻�� ---
                // �v���C���[1�̍��W�ƍU���}�X�̍��W����v���邩�`�F�b�N
                if (playerController1.GetCurrentX() == attackX && playerController1.GetCurrentY() == attackY)
                {
                    // ��v�����ꍇ�A�v���C���[1�ɃX�^�����ʂ�K�p
                    playerController1.ApplyStun(stunDuration);
                }

                // --- �v���C���[2�̓����蔻�� ---
                // �v���C���[2�̍��W�ƍU���}�X�̍��W����v���邩�`�F�b�N
                if (playerController2.GetCurrentX() == attackX && playerController2.GetCurrentY() == attackY)
                {
                    // ��v�����ꍇ�A�v���C���[2�ɃX�^�����ʂ�K�p
                    playerController2.ApplyStun(stunDuration);
                }
            }

            // 5. �N�[���^�C��
            // ���̍U���T�C�N���܂ł̑ҋ@���Ԃ��v�Z���đ҂�
            float cooldownTime = interval - displayDuration;
            if (cooldownTime > 0)
            {
                yield return new WaitForSeconds(cooldownTime);
            }
        }
    }
}