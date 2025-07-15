using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCountUP : MonoBehaviour
{
    [Header("報酬表示用のスコア画像")]
    [SerializeField] private ScoreImage battleRewardImage;

    // カウントアップの目標値
    private int targetBattleRewardCount;

    // カウントアップ演出の時間
    [SerializeField] private float resultDuration = 0.4f;

    // コルーチン用
    private Coroutine resultCoroutine;

    /// <summary>
    /// 外部から呼び出す: カウントアップ開始
    /// </summary>
    public void StartResult(int targetCount)
    {
        targetBattleRewardCount = targetCount;

        // 既に回ってたら止める
        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
        }

        resultCoroutine = StartCoroutine(Result());
    }

    /// <summary>
    /// カウントアップ演出
    /// </summary>
    private IEnumerator Result()
    {
        float elapsedTime = 0f;
        int count = 0;

        while (true)
        {
            elapsedTime += Time.deltaTime;

            count = Mathf.FloorToInt(Mathf.Lerp(0, targetBattleRewardCount, elapsedTime / resultDuration));

            // スプライトで数字を表示
            battleRewardImage.ShowNumber(count);

            if (elapsedTime >= resultDuration || Input.GetMouseButtonDown(0))
            {
                // 最終値で止める
                battleRewardImage.ShowNumber(targetBattleRewardCount);
                break;
            }

            yield return null;
        }
    }
}
