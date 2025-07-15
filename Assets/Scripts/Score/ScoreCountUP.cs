using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCountUP : MonoBehaviour
{
    [Header("��V�\���p�̃X�R�A�摜")]
    [SerializeField] private ScoreImage battleRewardImage;

    // �J�E���g�A�b�v�̖ڕW�l
    private int targetBattleRewardCount;

    // �J�E���g�A�b�v���o�̎���
    [SerializeField] private float resultDuration = 0.4f;

    // �R���[�`���p
    private Coroutine resultCoroutine;

    /// <summary>
    /// �O������Ăяo��: �J�E���g�A�b�v�J�n
    /// </summary>
    public void StartResult(int targetCount)
    {
        targetBattleRewardCount = targetCount;

        // ���ɉ���Ă���~�߂�
        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
        }

        resultCoroutine = StartCoroutine(Result());
    }

    /// <summary>
    /// �J�E���g�A�b�v���o
    /// </summary>
    private IEnumerator Result()
    {
        float elapsedTime = 0f;
        int count = 0;

        while (true)
        {
            elapsedTime += Time.deltaTime;

            count = Mathf.FloorToInt(Mathf.Lerp(0, targetBattleRewardCount, elapsedTime / resultDuration));

            // �X�v���C�g�Ő�����\��
            battleRewardImage.ShowNumber(count);

            if (elapsedTime >= resultDuration || Input.GetMouseButtonDown(0))
            {
                // �ŏI�l�Ŏ~�߂�
                battleRewardImage.ShowNumber(targetBattleRewardCount);
                break;
            }

            yield return null;
        }
    }
}
