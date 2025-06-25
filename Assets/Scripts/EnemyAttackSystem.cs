using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �G�̍U���p�^�[���i�\�����U���j���Ǘ�����V�X�e���ł��B
/// </summary>
public class EnemyAttackSystem : MonoBehaviour
{
    [Header("�� �֘A�I�u�W�F�N�g�̐ݒ�")]
    [Tooltip("�V�[���ɔz�u����9�̒��Ӄ}�[�N�I�u�W�F�N�g")]
    public List<GameObject> attentionMarks;
    [Tooltip("�U�����ɕ\������G�t�F�N�g�̃v���n�u")]
    public GameObject attackEffectPrefab;
    [Tooltip("�U���ΏۂƂȂ�v���C���[�̃R���g���[���[")]
    public PlayerController playerController;

    [Header("�� �U���^�C�~���O�̐ݒ�")]
    [Tooltip("���̍U�����n�܂�܂ł̎����i�b�j")]
    public float interval = 10f;
    [Tooltip("���Ӄ}�[�N���\������Ă��鎞�ԁi�\�����ԁj")]
    public float displayDuration = 2f;
    [Tooltip("�����ɍU������}�X�̐�")]
    public int glowCount = 3;
    [Tooltip("�U�����q�b�g�����ۂ̃X�^�����ԁi�b�j")]
    public float stunDuration = 1.5f;


    /// <summary>
    /// �Q�[���J�n���̏���������
    /// </summary>
    void Start()
    {
        // �C���X�y�N�^�[�̐ݒ肪�s�����Ă��Ȃ����`�F�b�N
        if (attentionMarks == null || attentionMarks.Count != 9 || attackEffectPrefab == null || playerController == null)
        {
            Debug.LogError("AttentionSystem�̐ݒ肪�s�����Ă��܂��I�C���X�y�N�^�[���m�F���Ă��������B");
            enabled = false;
            return;
        }
        // �Q�[���J�n���ɑS�Ă̒��Ӄ}�[�N���\���ɂ���
        foreach (GameObject mark in attentionMarks)
        {
            if (mark != null) mark.SetActive(false);
        }
        // �U�����[�v���J�n
        StartCoroutine(AttackLoop());
    }

    /// <summary>
    /// �U���T�C�N���𖳌��ɌJ��Ԃ��R���[�`��
    /// </summary>
    private IEnumerator AttackLoop()
    {
        while (true)
        {
            // 1. �U������}�X�������_���ɑI��
            List<int> chosenIndices = Enumerable.Range(0, 9).OrderBy(x => Random.value).Take(glowCount).ToList();

            // 2. �U���́u�\���v���s���i���Ӄ}�[�N���I���ɂ���j
            foreach (int index in chosenIndices)
            {
                attentionMarks[index].SetActive(true);
            }

            // 3. �\�����ԁi�v���C���[�̉�����ԁj�����ҋ@
            yield return new WaitForSeconds(displayDuration);

            // 4. �U���́u���s�v�Ɓu�q�b�g����v
            foreach (int index in chosenIndices)
            {
                // �܂��A�\���Ɏg�������Ӄ}�[�N���I�t�ɂ���
                attentionMarks[index].SetActive(false);

                // �U���G�t�F�N�g���A�\���Ɠ����ꏊ�ɕ\������
                Instantiate(attackEffectPrefab, attentionMarks[index].transform.position, Quaternion.identity);

                // --- �������炪�_���[�W����̊j�S���� ---
                // �U���}�X�̘_�����W���v�Z (��: �C���f�b�N�X5 -> X:2, Y:1)
                int attackX = index % 3;
                int attackY = index / 3;

                // �v���C���[�̌��݂̘_�����W���擾
                int playerX = playerController.GetCurrentX();
                int playerY = playerController.GetCurrentY();

                // �U���}�X�̍��W�ƃv���C���[�̍��W�����S�Ɉ�v���Ă��邩�`�F�b�N
                if (attackX == playerX && attackY == playerY)
                {
                    // ��v���Ă���΁A�v���C���[�Ɂu�X�^������v�Ɩ��߂��o��
                    playerController.ApplyStun(stunDuration);
                }
            }

            // 5. ���̍U���܂ł́u�N�[���^�C���v
            float cooldownTime = interval - displayDuration;
            if (cooldownTime > 0)
            {
                yield return new WaitForSeconds(cooldownTime);
            }
        }
    }
}