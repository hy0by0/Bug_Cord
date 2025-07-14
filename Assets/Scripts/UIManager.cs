using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UIを扱うため必須。

/// <summary>
/// UIにまつわる処理を担う。EnemyControllerから呼び出されて使われる。
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("■ 関連UIオブジェクト")]
    [Tooltip("HPバーとして使う、Imageコンポーネントが付いたGameObject")]
    public GameObject enemy_hp_bar;

    [Header("■ HPデータ（EnemyControllerから設定されます）")]
    // EnemyControllerが、対応するこのUIManagerにHPの最大値を設定するための変数
    public int enemy_hp_max;
    // EnemyControllerが、対応するこのUIManagerに現在のHPを設定するための変数
    public float enemy_hp_remain;

    [Header("■ スコア管理")]
    [Tooltip("ScoreManagerへの参照")]
    [SerializeField] ScoreManager score_manager;


    /// <summary>
    /// 敵のHPに合わせてバーの長さを変更する。入力は受けたダメージ量。
    /// EnemyControllerのHitメソッドから呼び出される。
    /// </summary>
    public void DamagedBar(int damageAmount)
    {
        // 自身の管理している残りHPから、受け取ったダメージ量を引く
        enemy_hp_remain -= damageAmount;

        // ScoreManagerが存在すれば、スコアを加算
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.getScore += damageAmount;
        }
        else
        {
            Debug.LogWarning("ScoreManager Instance が見つかりません");
        }

        // HPバーの表示を更新
        if (enemy_hp_bar != null)
        {
            Image hpBarImage = enemy_hp_bar.GetComponent<Image>();
            if (hpBarImage != null && enemy_hp_max > 0)
            {
                // (現在のHP / 最大HP) で割合を計算し、バーの長さに反映
                float gauge = enemy_hp_remain / enemy_hp_max;
                hpBarImage.fillAmount = gauge;
            }
        }
    }
}