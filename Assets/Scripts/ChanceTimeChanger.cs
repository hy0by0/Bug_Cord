using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChanceTimeChanger : MonoBehaviour
{
    public static ChanceTimeChanger Instance { get; private set; }

    [Header("■ ラリー設定")]
    public int requiredHitsForChanceTime = 10;
    public string chanceTimeSceneName = "SampleScene";

    [Header("■ 関連オブジェクト（Mainシーンで設定）")]
    public EnemyController enemyP1_ref;
    public EnemyController enemyP2_ref;

    private int p1HealthBeforeChance;
    private int p2HealthBeforeChance;
    public static int DamageDealtInChanceTime = 0;

    private int totalCriticalHits = 0;
    private bool isRallyActive = false;
    private bool isSceneTransitioning = false;
    // ★【追加】チャンスタイムに一度でも行ったかを記録するフラグ
    private bool hasBeenInChanceTime = false;

    public bool IsRallyActive => isRallyActive;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ★【変更】Mainシーンに戻り、かつ、チャンスタイムに行ったことがある場合のみ復元
        if (scene.name == "Main" && hasBeenInChanceTime)
        {
            StartCoroutine(RestoreEnemiesState());
            // ★【追加】復元処理が終わったら、フラグをリセットして次回のチャンスタイムに備える
            hasBeenInChanceTime = false;
        }
    }

    private IEnumerator RestoreEnemiesState()
    {
        yield return null;

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        EnemyController p1 = enemies.FirstOrDefault(e => e.playerID == PlayerID.P1);
        EnemyController p2 = enemies.FirstOrDefault(e => e.playerID == PlayerID.P2);

        if (p1 != null && p2 != null)
        {
            Debug.Log("敵の状態を復元します...");
            p1.SetHealth(p1HealthBeforeChance);
            p2.SetHealth(p2HealthBeforeChance);

            int sharedDamage = DamageDealtInChanceTime / 2;
            Debug.Log($"チャンスタイムボーナスダメージ: {sharedDamage} を両方に適用！");
            p1.TakeDamage(sharedDamage);
            p2.TakeDamage(sharedDamage);

            enemyP1_ref = p1;
            enemyP2_ref = p2;
        }

        DamageDealtInChanceTime = 0;
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

        if (totalCriticalHits >= requiredHitsForChanceTime)
        {
            StartCoroutine(TransitionToChanceTime());
            return;
        }

        PlayerID nextTargetID = (idOfEnemyJustHit == PlayerID.P1) ? PlayerID.P2 : PlayerID.P1;
        SwitchCriticalTarget(nextTargetID);
    }

    /// <summary>
    /// ラリーが終了したことを通知する
    /// </summary>
    public void EndRally()
    {
        isRallyActive = false;
    }

    private void SwitchCriticalTarget(PlayerID targetID)
    {
        if (targetID == PlayerID.P1 && enemyP1_ref != null)
        {
            enemyP1_ref.ShowCriticalPoint();
            if (enemyP2_ref != null) enemyP2_ref.HideCriticalPoint();
        }
        else if (targetID == PlayerID.P2 && enemyP2_ref != null)
        {
            enemyP2_ref.ShowCriticalPoint();
            if (enemyP1_ref != null) enemyP1_ref.HideCriticalPoint();
        }
    }

    private IEnumerator TransitionToChanceTime()
    {
        isSceneTransitioning = true;

        if (enemyP1_ref != null) p1HealthBeforeChance = enemyP1_ref.enemy_hp;
        if (enemyP2_ref != null) p2HealthBeforeChance = enemyP2_ref.enemy_hp;

        DamageDealtInChanceTime = 0;

        // ★【追加】チャンスタイムに行く直前に、フラグを立てる
        hasBeenInChanceTime = true;

        Debug.Log($"HPを保存: P1={p1HealthBeforeChance}, P2={p2HealthBeforeChance}");
        Debug.Log("目標達成！チャンスタイムに移行します。");

        EndRally();

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