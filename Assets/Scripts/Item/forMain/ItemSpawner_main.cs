using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner_main : MonoBehaviour
{
    [Header("■ プレイヤー設定")]
    public PlayerID playerID;
    public PlayerController myPlayer;

    [Header("■ 配置設定")]
    public List<Transform> positionMarkers;
    public List<GameObject> itemPrefabs;

    [Header("■ 弱点ヒット設定")]
    public int weakHitThreshold = 5;
    private int weakHitCount = 0;

    [Header("■ 制限設定")]
    public int maxItems = 3;
    public float itemLifetime = 10.0f;

    [Header("■ 遠近法（パース）設定")]
    public float frontRowScale = 1.0f;
    public float backRowScale = 0.7f;

    private Dictionary<Vector2Int, GameObject> spawnedItems = new Dictionary<Vector2Int, GameObject>();

    // PlayerID → スポナー インスタンス を保持する辞書
    private static Dictionary<PlayerID, ItemSpawner_main> spawners = new Dictionary<PlayerID, ItemSpawner_main>();

    private void Awake()
    {
        // シングルトンではなく辞書で管理
        spawners[playerID] = this;
        Debug.Log($"[ItemSpawner] Registered spawner for {playerID}");
    }

    /// <summary>
    /// EnemyHitArea2D から呼び出し。弱点ヒットをカウントし、閾値到達でスポーン
    /// </summary>
    public static void NotifyWeakHit(PlayerID hitter)
    {
        // 攻撃したプレイヤーの逆側にスポーン
        PlayerID targetID = (hitter == PlayerID.P1) ? PlayerID.P2 : PlayerID.P1;
        if (spawners.TryGetValue(targetID, out var spawner))
        {
            spawner.HandleWeakHit();
        }
        else
        {
            Debug.LogError($"[ItemSpawner] No spawner found for {targetID}");
        }
    }

    private void HandleWeakHit()
    {
        weakHitCount++;
        Debug.Log($"[ItemSpawner] {playerID} weakHits={weakHitCount}/{weakHitThreshold}");
        if (weakHitCount >= weakHitThreshold)
        {
            weakHitCount = 0;
            SpawnOneItem();
        }
    }

    /// <summary>
    /// ランダム空きマスに１つだけアイテムをスポーン
    /// </summary>
    private void SpawnOneItem()
    {
        if (spawnedItems.Count >= maxItems)
        {
            return;
        }

        // 空き座標を収集
        var occupied = new HashSet<Vector2Int>();
        occupied.Add(new Vector2Int(myPlayer.GetCurrentX(), myPlayer.GetCurrentY()));
        foreach (var kv in spawnedItems)
            occupied.Add(kv.Key);

        var empty = new List<int>();
        for (int i = 0; i < positionMarkers.Count; i++)
        {
            var coord = new Vector2Int(i % 3, i / 3);
            if (!occupied.Contains(coord)) empty.Add(i);
        }
        if (empty.Count == 0) return;

        // ランダム選択＆生成
        int idx = empty[Random.Range(0, empty.Count)];
        var marker = positionMarkers[idx];
        int y = idx / 3;
        int x = idx % 3;
        var prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
        var newItem = Instantiate(prefab, marker.position, Quaternion.identity);

        // 初期化
        var ctrl = newItem.GetComponent<ItemController_main>();
        if (ctrl != null) ctrl.Initialize(playerID, this);

        // スケーリング
        float t = y / 2f;
        newItem.transform.localScale *= Mathf.Lerp(frontRowScale, backRowScale, t);

        var coordKey = new Vector2Int(x, y);
        spawnedItems[coordKey] = newItem;
        Debug.Log($"[ItemSpawner] Spawned item at {coordKey} for {playerID}");

        StartCoroutine(DestroyAfterLifetime(newItem, coordKey));
    }

    private IEnumerator DestroyAfterLifetime(GameObject item, Vector2Int coords)
    {
        yield return new WaitForSeconds(itemLifetime);
        if (spawnedItems.TryGetValue(coords, out var obj) && obj == item)
        {
            Destroy(item);
            spawnedItems.Remove(coords);
            Debug.Log($"[ItemSpawner] Destroyed expired item at {coords}");
        }
    }
}
