using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 敵の攻撃パターン（予告→攻撃）を管理するシステム。
/// 3x3のマスに対してランダムに攻撃を仕掛けます。
/// </summary>
public class EnemyAttackSystem : MonoBehaviour
{
    [Header("■ 関連オブジェクトの設定")]
    [Tooltip("攻撃予告を表示する9個のUIオブジェクト")]
    public List<GameObject> attentionMarks;
    [Tooltip("攻撃時に生成するエフェクトのプレハブ")]
    public GameObject attackEffectPrefab;
    [Tooltip("攻撃対象となるプレイヤー1")]
    public PlayerController playerController1;
    [Tooltip("攻撃対象となるプレイヤー2")]
    public PlayerController playerController2;

    [Header("■ 攻撃パラメータ")]
    [Tooltip("次の攻撃予告が始まるまでの間隔（秒）")]
    public float interval = 10f;
    [Tooltip("攻撃予告が表示されてから攻撃が発生するまでの時間（秒）")]
    public float displayDuration = 2f;
    [Tooltip("一度に攻撃するマスの数")]
    public int glowCount = 3;
    [Tooltip("攻撃がヒットした際のスタン時間（秒）")]
    public float stunDuration = 1.5f;

    /// <summary>
    /// コンポーネント初期化時の処理
    /// </summary>
    void Start()
    {
        // Inspectorでの設定が正しいか検証
        if (attentionMarks == null || attentionMarks.Count != 9 || attackEffectPrefab == null || playerController1 == null || playerController2 == null)
        {
            Debug.LogError("EnemyAttackSystemのインスペクター設定が不足しています。オブジェクトをアタッチしてください。");
            enabled = false; // エラー時はコンポーネントを無効化
            return;
        }

        // ゲーム開始時に全ての予告を非表示にする
        foreach (GameObject mark in attentionMarks)
        {
            if (mark != null) mark.SetActive(false);
        }

        // 攻撃のメインループを開始
        StartCoroutine(AttackLoop());
    }

    /// <summary>
    /// 「予告→待機→攻撃」のサイクルを無限に繰り返すコルーチン
    /// </summary>
    private IEnumerator AttackLoop()
    {
        while (true)
        {
            // 1. 攻撃マスの選定
            // 0から8のインデックスをランダムに並べ替え、先頭から指定個数分だけ選択する
            List<int> chosenIndices = Enumerable.Range(0, 9).OrderBy(x => Random.value).Take(glowCount).ToList();

            // 2. 攻撃の予告
            // 選んだマスの予告をアクティブにする
            foreach (int index in chosenIndices)
            {
                attentionMarks[index].SetActive(true);
            }

            // 3. プレイヤーの回避時間
            // 予告が表示されている間、待機する
            yield return new WaitForSeconds(displayDuration);

            // 4. 攻撃の実行と当たり判定
            foreach (int index in chosenIndices)
            {
                // 攻撃発生と同時に予告を非表示にする
                attentionMarks[index].SetActive(false);

                // 攻撃エフェクトを予告と同じ位置に生成
                Instantiate(attackEffectPrefab, attentionMarks[index].transform.position, Quaternion.identity);

                // 攻撃マスのグリッド座標を計算 (例: インデックス5 -> X:2, Y:1)
                int attackX = index % 3;
                int attackY = index / 3;

                // --- プレイヤー1の当たり判定 ---
                // プレイヤー1の座標と攻撃マスの座標が一致するかチェック
                if (playerController1.GetCurrentX() == attackX && playerController1.GetCurrentY() == attackY)
                {
                    // 一致した場合、プレイヤー1にスタン効果を適用
                    playerController1.ApplyStun(stunDuration);
                }

                // --- プレイヤー2の当たり判定 ---
                // プレイヤー2の座標と攻撃マスの座標が一致するかチェック
                if (playerController2.GetCurrentX() == attackX && playerController2.GetCurrentY() == attackY)
                {
                    // 一致した場合、プレイヤー2にスタン効果を適用
                    playerController2.ApplyStun(stunDuration);
                }
            }

            // 5. クールタイム
            // 次の攻撃サイクルまでの待機時間を計算して待つ
            float cooldownTime = interval - displayDuration;
            if (cooldownTime > 0)
            {
                yield return new WaitForSeconds(cooldownTime);
            }
        }
    }
}