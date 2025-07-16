using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ★【修正】消えていたクラス定義を元に戻しました
public enum EnemyState
{
    Normal, Stan, Alert, Dead, Attack, Cooldown, Special
}
[System.Serializable]
public class CriticalHitBox_Spawnded
{
    public string pairID;
    public GameObject hitboxObject;
}


public class EnemyController : MonoBehaviour
{
    [Header("■ チーム設定")]
    public PlayerID playerID;

    [Header("■ グラフィック関連")]
    [SerializeField] private GameObject criticalEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    [Header("■ ステータス")]
    // ★ 'static' を削除し、各敵が個別のHPと状態を持つようにします
    public int enemy_hp = 10000;
    public string enemy_state = "Normal";

    [SerializeField] UIManager uimanager;

    [Header("■ クリティカルヒット関連")]
    public int count_hit = 0;
    public int count_hit_Flag = 5;
    public List<CriticalHitBox_Spawnded> spawnPairs;
    public float activeDuration_HitBox = 8f;

    // --- 内部変数 ---
    private bool hasSpawned = false;
    private bool rallyStartRequested = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // UIManagerの仕様に合わせて初期化
        if (uimanager != null)
        {
            uimanager.enemy_hp_max = this.enemy_hp;
            uimanager.enemy_hp_remain = this.enemy_hp;
        }
        count_hit = 0;
    }

    void Update()
    {
        if (enemy_state == "Dead") return;

        string currentSceneName = SceneManager.GetActiveScene().name;

        // --- ChanceTime(SampleScene)の場合：従来の攻撃回数でクリティカルが出現 ---
        if (currentSceneName == "SampleScene")
        {
            if (!hasSpawned && count_hit >= count_hit_Flag)
            {
                SpawnPairByID("1");
            }
        }
        // --- Mainシーンの場合：ラリー形式のクリティカル ---
        else if (currentSceneName == "Main")
        {
            if (ChanceTimeChanger.Instance != null && !ChanceTimeChanger.Instance.IsRallyActive && !rallyStartRequested && count_hit >= count_hit_Flag)
            {
                rallyStartRequested = true;
                ChanceTimeChanger.Instance.StartRally(this.playerID);
            }
        }

        if (enemy_hp <= 0)
        {
            enemy_state = "Dead";
            if (ChanceTimeChanger.Instance != null && ChanceTimeChanger.Instance.IsRallyActive)
            {
                ChanceTimeChanger.Instance.EndRally();
            }
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            // Mainシーンに戻ったらフラグとヒットカウントをリセット
            rallyStartRequested = false;
            count_hit = 0;
        }
    }

    // --- ヒット処理メソッド群 ---

    public void HitResist(int playerAtackdamage)
    {
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 0.5f);
        enemy_hp -= enemy_damaged;
        if (uimanager != null) uimanager.DamagedBar(enemy_damaged);
        count_hit++;
    }

    public void HitNormal(int playerAtackdamage)
    {
        // ヒットエフェクトなどの処理

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashColorCoroutine());

        // ★ 1. まず、ここでダメージ量を計算し、ローカル変数 enemy_damaged を宣言（作成）する
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.0f);

        // ★ 2. 作成した enemy_damaged 変数を、引数として HandleDamage に渡す
        HandleDamage(enemy_damaged);

        // 3. ヒット回数をカウントアップ
        count_hit++;
    }

    public void HitWeak(int playerAtackdamage)
    {
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.1f);
        enemy_hp -= enemy_damaged;
        if (uimanager != null) uimanager.DamagedBar(enemy_damaged);
        HandleDamage(enemy_damaged);
        count_hit++;
    }

    public void HitCritical(int playerAtackdamage)
    {
        Instantiate(criticalEffectPrefab, transform.position, Quaternion.identity);
        int enemy_damaged = Mathf.RoundToInt(playerAtackdamage * 1.3f);
        enemy_hp -= enemy_damaged;
        if (uimanager != null) uimanager.DamagedBar(enemy_damaged);

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Main" && ChanceTimeChanger.Instance != null)
        {
            HideCriticalPoint();
            ChanceTimeChanger.Instance.ReportCriticalHitAndSwitch(this.playerID);
        }
        enemy_state = "stan";

        HandleDamage(enemy_damaged);
        count_hit++;
    }

        /// <summary>
    /// ダメージ関連の共通処理を行うメソッド
    /// </summary>
    private void HandleDamage(int damage) // damage という名前で enemy_damaged の値を受け取る
    {
        // 自身のHPを減らす
        enemy_hp -= damage;

        // UIマネージャーに通知する
        if (uimanager != null)
        {
            uimanager.DamagedBar(damage);
        }

        // もし現在のシーンがチャンスタイムなら、与えたダメージを監督に報告する
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            if (ChanceTimeChanger.Instance != null)
            {
                ChanceTimeChanger.DamageDealtInChanceTime += damage;
            }
        }
    }

    /// <summary>
    /// 外部からHPを直接設定するためのメソッド (ChanceTimeChangerが使用)
    /// </summary>
    public void SetHealth(int newHealth)
    {
        Debug.Log($"[{playerID}] SetHealthが呼ばれました。新しいHP: {newHealth}");

        // 1. この敵のHPの値を、受け取った新しい値に更新する
        enemy_hp = newHealth;

        // 2. UIManagerにも最新の状態を教える
        if (uimanager != null)
        {
            // UIManagerが持っている残りHPの変数も、新しい値に直接設定する
            uimanager.enemy_hp_remain = newHealth;

            // ★ UIManagerの仕様上、DamagedBarを呼ばないとHPバーが更新されないため、
            //    ダメージ量0で呼び出して、表示だけを強制的に更新させる
            uimanager.DamagedBar(0);
        }
    }

    /// <summary>
    /// 外部からダメージを直接与えるためのメソッド (ChanceTimeChangerが使用)
    /// </summary>
    public void TakeDamage(int damage)
    {
        enemy_hp -= damage;
        if (uimanager != null)
        {
            uimanager.DamagedBar(damage);
        }
    }

    // --- クリティカルポイント関連メソッド ---

    public void SpawnPairByID(string id)
    {
        if (hasSpawned) return;
        CriticalHitBox_Spawnded targetPair = spawnPairs.Find(p => p.pairID == id);
        if (targetPair != null)
        {
            StartCoroutine(SpawnHitboxCoroutine(targetPair));
        }
    }

    private IEnumerator SpawnHitboxCoroutine(CriticalHitBox_Spawnded pair)
    {
        hasSpawned = true;
        if (pair.hitboxObject != null) pair.hitboxObject.SetActive(true);
        yield return new WaitForSeconds(activeDuration_HitBox);
        if (pair.hitboxObject != null) pair.hitboxObject.SetActive(false);
        count_hit = 0;
        hasSpawned = false;
    }

    public void ShowCriticalPoint()
    {
        CriticalHitBox_Spawnded targetPair = spawnPairs.Find(p => p.pairID == "1");
        if (targetPair != null && targetPair.hitboxObject != null)
        {
            targetPair.hitboxObject.SetActive(true);
        }
    }

    public void HideCriticalPoint()
    {
        CriticalHitBox_Spawnded targetPair = spawnPairs.Find(p => p.pairID == "1");
        if (targetPair != null && targetPair.hitboxObject != null)
        {
            targetPair.hitboxObject.SetActive(false);
        }
    }

    private IEnumerator FlashColorCoroutine()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }
}
