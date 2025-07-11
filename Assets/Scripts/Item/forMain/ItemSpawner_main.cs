using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner_main : MonoBehaviour
{
    [Header("■ プレイヤー設定")]
    public PlayerID playerID;

    [Header("■ 関連オブジェクト")]
    public PlayerController myPlayer;

    [Header("■ 配置設定")]
    public List<Transform> positionMarkers;
    public List<GameObject> itemPrefabs;

    [Header("■ 時間設定")]
    public float spawnInterval = 15.0f;
    public float itemLifetime = 10.0f;
    public int maxItems = 3;

    [Header("■ 遠近法（パース）設定")]
    public float frontRowScale = 1.0f;
    public float backRowScale = 0.7f;

    private Dictionary<Vector2Int, GameObject> spawnedItems = new Dictionary<Vector2Int, GameObject>();

    // ★★★【追加】このStart()関数が抜けていました ★★★
    void Start()
    {
        // プレイヤーがインスペクターで設定されているかチェック
        if (myPlayer == null)
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

            if (spawnedItems.Count >= maxItems)
            {
                continue;
            }

            HashSet<Vector2Int> occupiedCoordinates = new HashSet<Vector2Int>();
            occupiedCoordinates.Add(new Vector2Int(myPlayer.GetCurrentX(), myPlayer.GetCurrentY()));
            foreach (Vector2Int itemCoords in spawnedItems.Keys)
            {
                occupiedCoordinates.Add(itemCoords);
            }

            List<int> availableIndexes = new List<int>();
            for (int i = 0; i < positionMarkers.Count; i++)
            {
                Vector2Int currentCoords = new Vector2Int(i % 3, i / 3);
                if (!occupiedCoordinates.Contains(currentCoords))
                {
                    availableIndexes.Add(i);
                }
            }

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

                ItemController_main itemController = newItem.GetComponent<ItemController_main>();
                if (itemController != null)
                {
                    itemController.Initialize(playerID, this);
                }

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

        if (spawnedItems.ContainsKey(coords) && spawnedItems[coords] == item)
        {
            Destroy(item);
            spawnedItems.Remove(coords);
        }
    }
}