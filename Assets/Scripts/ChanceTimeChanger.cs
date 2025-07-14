using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// クリティカルヒットのラリーを管理し、合計回数に応じてチャンスタイムへのシーン遷移を担う。
/// </summary>
public class ChanceTimeChanger : MonoBehaviour
{
    // このクラスの唯一のインスタンスを保持する（シングルトンパターン）
    public static ChanceTimeChanger Instance { get; private set; }

    [Header("■ ラリー設定")]
    [Tooltip("チャンスタイムに移行するために必要なヒット回数")]
    public int requiredHitsForChanceTime = 10;
    [Tooltip("遷移先のチャンスタイムシーン名")]
    public string chanceTimeSceneName = "ChanceTime";

    [Header("■ 関連オブジェクト")]
    [Tooltip("P1チームの敵")]
    public EnemyController enemyP1;
    [Tooltip("P2チームの敵")]
    public EnemyController enemyP2;

    // --- 内部変数 ---
    private int totalCriticalHits = 0;
    private bool isRallyActive = false;
    private bool isSceneTransitioning = false;

    // --- 公開プロパティ ---
    /// <summary>
    /// 外部からラリー中か確認できるようにする
    /// </summary>
    public bool IsRallyActive => isRallyActive;

    void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで存在させる
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// クリティカルラリーの開始を宣言する
    /// </summary>
    public void StartRally(PlayerID initialTargetID)
    {
        if (isRallyActive) return;

        isRallyActive = true;
        totalCriticalHits = 0;
        Debug.Log("クリティカルラリー開始！");

        // 最初のクリティカルポイントを出現させる
        SwitchCriticalTarget(initialTargetID);
    }

    /// <summary>
    /// クリティカルヒットを報告させ、ターゲットを切り替える
    /// </summary>
    public void ReportCriticalHitAndSwitch(PlayerID idOfEnemyJustHit)
    {
        if (!isRallyActive || isSceneTransitioning) return;

        totalCriticalHits++;
        Debug.Log($"クリティカルヒット！ 合計: {totalCriticalHits} / {requiredHitsForChanceTime}回");

        // 必要な回数に達したらチャンスタイムへ移行
        if (totalCriticalHits >= requiredHitsForChanceTime)
        {
            StartCoroutine(TransitionToChanceTime());
            return;
        }

        // 次のターゲットを決定して、クリティカルポイントを移動させる
        PlayerID nextTargetID = (idOfEnemyJustHit == PlayerID.P1) ? PlayerID.P2 : PlayerID.P1;
        SwitchCriticalTarget(nextTargetID);
    }

    /// <summary>
    /// 指定されたターゲットのクリティカルポイントを有効化する
    /// </summary>
    private void SwitchCriticalTarget(PlayerID targetID)
    {
        if (targetID == PlayerID.P1 && enemyP1 != null)
        {
            enemyP1.ShowCriticalPoint();
            if (enemyP2 != null) enemyP2.HideCriticalPoint();
        }
        else if (targetID == PlayerID.P2 && enemyP2 != null)
        {
            enemyP2.ShowCriticalPoint();
            if (enemyP1 != null) enemyP1.HideCriticalPoint();
        }
    }

    /// <summary>
    /// チャンスタイムシーンへの遷移処理を行うコルーチン
    /// </summary>
    private IEnumerator TransitionToChanceTime()
    {
        isSceneTransitioning = true;
        Debug.Log("目標達成！チャンスタイムに移行します。");

        ScreenFader screenFader = FindObjectOfType<ScreenFader>();
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.BackBlack());
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene(chanceTimeSceneName);
        isSceneTransitioning = false;
    }
}