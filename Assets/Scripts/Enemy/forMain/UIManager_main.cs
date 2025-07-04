using UnityEngine;
using UnityEngine.UI; // UIコンポーネントを扱うために必須

/// <summary>
/// HPバーの「表示」だけを担うシンプルなクラス。
/// 敵一体につき、このコンポーネントが1つずつ必要になる。
/// </summary>
public class UIManager_main : MonoBehaviour
{
    [Header("■ 関連UIオブジェクト")]
    [Tooltip("このUIが管理するHPバーのImageコンポーネント")]
    public Image enemyHpBarImage;

    /// <summary>
    /// 外部（EnemyController_main）から呼び出され、HPバーの表示を更新する公開メソッド
    /// </summary>
    /// <param name="maxHp">敵の最大HP</param>
    /// <param name="currentHp">敵の現在のHP</param>
    public void UpdateHealth(float maxHp, float currentHp)
    {
        // HPバーのImageが設定されていなければ、処理を中断
        if (enemyHpBarImage == null)
        {
            Debug.LogWarning("HPバーのImageが設定されていません。", this.gameObject);
            return;
        }

        // ゼロ除算を防ぐためのチェック
        if (maxHp > 0)
        {
            // (現在のHP / 最大HP)で、バーの長さを計算 (値は0.0から1.0の間になる)
            // ImageコンポーネントのFill Amountを操作することで、バーの長さを変える
            enemyHpBarImage.fillAmount = currentHp / maxHp;
        }
        else
        {
            // 最大HPが0以下の場合は、バーを空にする
            enemyHpBarImage.fillAmount = 0;
        }
    }
}