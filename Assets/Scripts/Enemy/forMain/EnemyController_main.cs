using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一体の敵に関する処理全般を担うクラス。
/// 敵ごとにこのコンポーネントが1つずつ必要になる。
/// </summary>
public class EnemyController_main : MonoBehaviour
{
    [Header("■ チーム設定")]
    [Tooltip("この敵がP1とP2のどちらに対応するかを設定")]
    public PlayerID playerID;

    [Header("■ グラフィック関連")]
    [SerializeField] private GameObject criticalEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    [Header("■ ステータス")]
    public int enemy_hp = 2000;
    // ★ この敵専用のUIマネージャーへの参照
    [SerializeField] private UIManager_main uimanager;

    // ★【重要】'static'を削除。これにより、各敵が自分自身の状態を持つようになる。
    public string enemy_state = "Normal";

    // ★ 最大HPを記憶しておくための内部変数
    private int maxHp;

    [Header("■ クリティカルヒット関連")]
    public int count_hit = 0;
    public int count_hit_Flag = 5;
    public List<CriticalHitBox_Spawnded> spawnPairs;
    public float activeDuration_HitBox = 8f;
    private bool hasSpawned = false;

    /// <summary>
    /// ゲーム開始時に一度だけ呼ばれる初期化処理
    /// </summary>
    void Start()
    {
        // 自身のコンポーネントを取得
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // ★ ゲーム開始時のHPを最大HPとして記憶しておく
        maxHp = enemy_hp;

        // ★ 対応するUIマネージャーに、HPの初期状態を伝える
        if (uimanager != null)
        {
            uimanager.UpdateHealth(maxHp, enemy_hp);
        }

        count_hit = 0;
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新処理
    /// </summary>
    void Update()
    {
        if (!hasSpawned && count_hit >= count_hit_Flag)
        {
            SpawnPairByID("1");
        }

        if (enemy_hp <= 0)
        {
            enemy_state = "Dead";
        }
    }

    // --- 各ヒット処理メソッド ---
    // （主な変更点は、ダメージを受けた後にUIへ通知する部分）

    public void HitResist(int playerAtackdamage)
    {
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 0.5f);
        enemy_hp -= enemy_damaged;
        // ★ ダメージ後の最新のHP状態をUIマネージャーに通知
        if (uimanager != null) uimanager.UpdateHealth(maxHp, enemy_hp);
        count_hit++;
    }

    public void HitNormal(int playerAtackdamage)
    {
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashColorCoroutine());

        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.0f);
        enemy_hp -= enemy_damaged;
        // ★ ダメージ後の最新のHP状態をUIマネージャーに通知
        if (uimanager != null) uimanager.UpdateHealth(maxHp, enemy_hp);
        count_hit++;
    }

    public void HitWeak(int playerAtackdamage)
    {
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.5f);
        enemy_hp -= enemy_damaged;
        // ★ ダメージ後の最新のHP状態をUIマネージャーに通知
        if (uimanager != null) uimanager.UpdateHealth(maxHp, enemy_hp);
        count_hit++;
    }

    public void HitCritical(int playerAtackdamage)
    {
        Instantiate(criticalEffectPrefab, transform.position, Quaternion.identity);
        animator.SetTrigger("Critical");
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 2.0f);
        enemy_hp -= enemy_damaged;
        // ★ ダメージ後の最新のHP状態をUIマネージャーに通知
        if (uimanager != null) uimanager.UpdateHealth(maxHp, enemy_hp);
        enemy_state = "stan";
    }

    // (その他のメソッドは変更なし)
    public void SpawnPairByID(string id) { /* ... */ }
    private IEnumerator SpawnHitboxCoroutine(CriticalHitBox_Spawnded pair) { /* ... */ yield return null; }
    private IEnumerator FlashColorCoroutine() { /* ... */ yield return null; }
}