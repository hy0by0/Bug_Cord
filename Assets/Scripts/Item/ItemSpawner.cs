using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーや他のアイテムの位置を考慮し、空いているマスにのみアイテムを生成する。
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [Header("■ 関連オブジェクト")]
    [Tooltip("プレイヤー1")]
    public PlayerController playerController1;
    [Tooltip("プレイヤー2")]
    public PlayerController playerController2;

    [Header("■ 配置設定")]
    [Tooltip("アイテムを配置する場所の目印となるTransformを9個設定します")]
    public List<Transform> positionMarkers;
    [Tooltip("出現させるアイテムのプレハブを複数設定できます")]
    public List<GameObject> itemPrefabs;

    [Header("■ 時間設定")]
    [Tooltip("アイテムが新しく配置されるまでの間隔（秒）")]
    public float spawnInterval = 15.0f;
    [Tooltip("出現したアイテムがシーンから自動で消えるまでの時間（秒）")]
    public float itemLifetime = 10.0f;
    [Tooltip("同時に存在できるアイテム数の上限")]
    public int maxItems = 3;

    [Header("■ 遠近法（パース）設定")]
    [Tooltip("一番手前のマスでのアイテムの大きさの倍率 (例: 1.0)")]
    public float frontRowScale = 1.0f;
    [Tooltip("一番奥のマスでのアイテムの大きさの倍率 (例: 0.7)")]
    public float backRowScale = 0.7f;

    // 生成されたアイテムとそのグリッド座標をセットで記憶する辞書
    private Dictionary<Vector2Int, GameObject> spawnedItems = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // プレイヤーがインスペクターで設定されているかチェック
        if (playerController1 == null || playerController2 == null)
        {
            Debug.LogError("ItemSpawnerにプレイヤーが設定されていません！インスペクターを確認してください。", this);
            enabled = false;
            return;
        }

        // アイテム生成の繰り返し処理を開始
        StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator SpawnItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 現在のアイテム数が上限に達していたら、生成しない
            if (spawnedItems.Count >= maxItems)
            {
                continue;
            }

            // --- 配置済みのマス（プレイヤーと既存アイテム）をリストアップ ---
            HashSet<Vector2Int> occupiedCoordinates = new HashSet<Vector2Int>();

            occupiedCoordinates.Add(new Vector2Int(playerController1.GetCurrentX(), playerController1.GetCurrentY()));
            occupiedCoordinates.Add(new Vector2Int(playerController2.GetCurrentX(), playerController2.GetCurrentY()));

            foreach (Vector2Int itemCoords in spawnedItems.Keys)
            {
                occupiedCoordinates.Add(itemCoords);
            }

            // --- 配置可能な「空きマス」を探す ---
            List<int> availableIndexes = new List<int>();
            for (int i = 0; i < positionMarkers.Count; i++)
            {
                Vector2Int currentCoords = new Vector2Int(i % 3, i / 3);
                if (!occupiedCoordinates.Contains(currentCoords))
                {
                    availableIndexes.Add(i);
                }
            }

            // --- もし空きマスが1つでもあれば、そこに生成 ---
            if (availableIndexes.Count > 0)
            {
                int randomIndexInList = Random.Range(0, availableIndexes.Count);
                int positionIndex = availableIndexes[randomIndexInList];

                Transform spawnPoint = positionMarkers[positionIndex];
                int spawnY = positionIndex / 3;
                int spawnX = positionIndex % 3;

                int itemIndex = Random.Range(0, itemPrefabs.Count);
                GameObject itemToSpawn = itemPrefabs[itemIndex];
                GameObject newItem = Instantiate(itemToSpawn, spawnPoint.position, Quaternion.identity);

                Vector3 originalScale = newItem.transform.localScale;
                float t = spawnY / 2.0f;
                float scaleMultiplier = Mathf.Lerp(frontRowScale, backRowScale, t);
                newItem.transform.localScale = originalScale * scaleMultiplier;

                Vector2Int newCoords = new Vector2Int(spawnX, spawnY);
                spawnedItems.Add(newCoords, newItem);

                StartCoroutine(ItemLifetimeCoroutine(newItem, newCoords, itemLifetime));
            }
        }
    }

    private IEnumerator ItemLifetimeCoroutine(GameObject item, Vector2Int coords, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        // 時間経過後、そのアイテムがまだ存在（誰にも取得されていない）していれば破壊
        if (spawnedItems.ContainsKey(coords) && spawnedItems[coords] == item)
        {
            Destroy(item);
            spawnedItems.Remove(coords);
        }
    }
}