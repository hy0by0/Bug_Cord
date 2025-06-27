using UnityEngine;

/// <summary>
/// プレイヤーの攻撃力を管理し、状況に応じた最終ダメージを計算します。
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("参照するコントローラー")]
    [Tooltip("プレイヤーの現在位置を管理しているPlayerControllerコンポーネント")]
    [SerializeField] private PlayerController playerController;

    [Header("基本ダメージ設定")]
    [Tooltip("基本となるダメージ量")]
    public int baseDamage = 10;

    [Header("マス目による攻撃倍率")]
    [Tooltip("手前の行(Y=0)にいる時の攻撃倍率")]
    public float frontRowMultiplier = 1.0f;
    [Tooltip("中央の行(Y=1)にいる時の攻撃倍率")]
    public float middleRowMultiplier = 1.1f;
    [Tooltip("奥の行(Y=2)にいる時の攻撃倍率")]
    public float backRowMultiplier = 1.2f;

    /// <summary>
    /// ゲーム開始時に一度だけ呼ばれる初期化処理
    /// </summary>
    void Start()
    {
        // playerControllerがインスペクターから設定されているか確認
        if (playerController == null)
        {
            Debug.LogError("PlayerControllerが設定されていません！インスペクターから設定してください。", this);
        }
    }

    /// <summary>
    /// 現在位置に応じた最終ダメージを計算して返します。
    /// 敵スクリプトなどが攻撃を受けた際に、この関数を呼び出してダメージ量を取得します。
    /// </summary>
    /// <returns>計算後のダメージ量（小数点切り捨て）</returns>
    public int GetCalculatedDamage()
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerControllerが参照できないため、基本ダメージを返します。");
            return baseDamage;
        }

        // PlayerControllerから現在のY座標（行）を取得
        int currentRow = playerController.GetCurrentY();
        float currentMultiplier = 1.0f;

        // Y座標に応じて適用する倍率を決定
        switch (currentRow)
        {
            case 0: currentMultiplier = frontRowMultiplier; break;
            case 1: currentMultiplier = middleRowMultiplier; break;
            case 2: currentMultiplier = backRowMultiplier; break;
        }

        // 仕様書通り、基本ダメージ × 倍率で計算し、小数点以下を切り捨てる
        int finalDamage = (int)(baseDamage * currentMultiplier);

        return finalDamage;
    }
}