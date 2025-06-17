using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 敵の攻撃パターン（予告→攻撃）を管理するシステムです。
/// </summary>
public class EnemyAttackSystem : MonoBehaviour
{
    [Header("■ 関連オブジェクトの設定")]
    [Tooltip("シーンに配置した9個の注意マークオブジェクト")]
    public List<GameObject> attentionMarks;
    [Tooltip("攻撃時に表示するエフェクトのプレハブ")]
    public GameObject attackEffectPrefab;
    [Tooltip("攻撃対象となるプレイヤーのコントローラー")]
    public PlayerController playerController;

    [Header("■ 攻撃タイミングの設定")]
    [Tooltip("次の攻撃が始まるまでの周期（秒）")]
    public float interval = 10f;
    [Tooltip("注意マークが表示されている時間（予告時間）")]
    public float displayDuration = 2f;
    [Tooltip("同時に攻撃するマスの数")]
    public int glowCount = 3;
    [Tooltip("攻撃がヒットした際のスタン時間（秒）")]
    public float stunDuration = 1.5f;


    /// <summary>
    /// ゲーム開始時の初期化処理
    /// </summary>
    void Start()
    {
        // インスペクターの設定が不足していないかチェック
        if (attentionMarks == null || attentionMarks.Count != 9 || attackEffectPrefab == null || playerController == null)
        {
            Debug.LogError("AttentionSystemの設定が不足しています！インスペクターを確認してください。");
            enabled = false;
            return;
        }
        // ゲーム開始時に全ての注意マークを非表示にする
        foreach (GameObject mark in attentionMarks)
        {
            if (mark != null) mark.SetActive(false);
        }
        // 攻撃ループを開始
        StartCoroutine(AttackLoop());
    }

    /// <summary>
    /// 攻撃サイクルを無限に繰り返すコルーチン
    /// </summary>
    private IEnumerator AttackLoop()
    {
        while (true)
        {
            // 1. 攻撃するマスをランダムに選択
            List<int> chosenIndices = Enumerable.Range(0, 9).OrderBy(x => Random.value).Take(glowCount).ToList();

            // 2. 攻撃の「予告」を行う（注意マークをオンにする）
            foreach (int index in chosenIndices)
            {
                attentionMarks[index].SetActive(true);
            }

            // 3. 予告時間（プレイヤーの回避時間）だけ待機
            yield return new WaitForSeconds(displayDuration);

            // 4. 攻撃の「実行」と「ヒット判定」
            foreach (int index in chosenIndices)
            {
                // まず、予告に使った注意マークをオフにする
                attentionMarks[index].SetActive(false);

                // 攻撃エフェクトを、予告と同じ場所に表示する
                Instantiate(attackEffectPrefab, attentionMarks[index].transform.position, Quaternion.identity);

                // --- ここからがダメージ判定の核心部分 ---
                // 攻撃マスの論理座標を計算 (例: インデックス5 -> X:2, Y:1)
                int attackX = index % 3;
                int attackY = index / 3;

                // プレイヤーの現在の論理座標を取得
                int playerX = playerController.GetCurrentX();
                int playerY = playerController.GetCurrentY();

                // 攻撃マスの座標とプレイヤーの座標が完全に一致しているかチェック
                if (attackX == playerX && attackY == playerY)
                {
                    // 一致していれば、プレイヤーに「スタンしろ」と命令を出す
                    playerController.ApplyStun(stunDuration);
                }
            }

            // 5. 次の攻撃までの「クールタイム」
            float cooldownTime = interval - displayDuration;
            if (cooldownTime > 0)
            {
                yield return new WaitForSeconds(cooldownTime);
            }
        }
    }
}