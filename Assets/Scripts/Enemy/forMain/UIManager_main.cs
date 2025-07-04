using UnityEngine;
using UnityEngine.UI; // UI�R���|�[�l���g���������߂ɕK�{

/// <summary>
/// HP�o�[�́u�\���v������S���V���v���ȃN���X�B
/// �G��̂ɂ��A���̃R���|�[�l���g��1���K�v�ɂȂ�B
/// </summary>
public class UIManager_main : MonoBehaviour
{
    [Header("�� �֘AUI�I�u�W�F�N�g")]
    [Tooltip("����UI���Ǘ�����HP�o�[��Image�R���|�[�l���g")]
    public Image enemyHpBarImage;

    /// <summary>
    /// �O���iEnemyController_main�j����Ăяo����AHP�o�[�̕\�����X�V������J���\�b�h
    /// </summary>
    /// <param name="maxHp">�G�̍ő�HP</param>
    /// <param name="currentHp">�G�̌��݂�HP</param>
    public void UpdateHealth(float maxHp, float currentHp)
    {
        // HP�o�[��Image���ݒ肳��Ă��Ȃ���΁A�����𒆒f
        if (enemyHpBarImage == null)
        {
            Debug.LogWarning("HP�o�[��Image���ݒ肳��Ă��܂���B", this.gameObject);
            return;
        }

        // �[�����Z��h�����߂̃`�F�b�N
        if (maxHp > 0)
        {
            // (���݂�HP / �ő�HP)�ŁA�o�[�̒������v�Z (�l��0.0����1.0�̊ԂɂȂ�)
            // Image�R���|�[�l���g��Fill Amount�𑀍삷�邱�ƂŁA�o�[�̒�����ς���
            enemyHpBarImage.fillAmount = currentHp / maxHp;
        }
        else
        {
            // �ő�HP��0�ȉ��̏ꍇ�́A�o�[����ɂ���
            enemyHpBarImage.fillAmount = 0;
        }
    }
}