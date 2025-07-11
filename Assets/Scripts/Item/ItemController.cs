using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各アイテムが持つ「自分の情報」を管理し、ItemGetSystemに自身の存在を通知する。
/// </summary>
public class ItemController : MonoBehaviour
{
    public enum ItemType { SpecialMachineGun, SwiftBoots, EnergyCider }
    [Header("アイテム設定")]
    public ItemType itemType;
    public float effectDuration = 8.0f;

    private Vector2Int myGridCoords;
    private ItemGetSystem itemGetSystem;

    void Start()
    {
        // 司令塔であるItemGetSystemをシーンから見つける
        itemGetSystem = FindObjectOfType<ItemGetSystem>();
        if (itemGetSystem == null)
        {
            Destroy(gameObject); // 司令塔がいないなら消滅
            return;
        }

        // 自分のグリッド座標を計算して特定する
        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        if (spawner != null)
        {
            Transform closestMarker = null;
            float minDistance = float.MaxValue;
            foreach (Transform marker in spawner.positionMarkers)
            {
                float distance = Vector3.Distance(transform.position, marker.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestMarker = marker;
                }
            }
            int index = spawner.positionMarkers.IndexOf(closestMarker);
            myGridCoords = new Vector2Int(index % 3, index / 3);

            // 司令塔に自分の存在を登録
            itemGetSystem.RegisterItem(myGridCoords, this);
        }
    }

    /// <summary>
    /// オブジェクトが破壊される時に自動で呼ばれる
    /// </summary>
    void OnDestroy()
    {
        // 司令塔がまだ存在するなら、管理リストから自分の情報を削除してもらう
        if (itemGetSystem != null)
        {
            itemGetSystem.UnregisterItem(myGridCoords);
        }
    }
}