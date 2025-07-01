using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン上のアイテムとプレイヤーを監視し、アイテム取得を管理する司令塔。
/// </summary>
public class ItemGetSystem : MonoBehaviour
{
    [Header("■ 監視対象のプレイヤー")]
    public PlayerController player1;
    public PlayerController player2;

    // シーンに存在する全アイテムの座標と情報を管理するリスト
    private Dictionary<Vector2Int, ItemController> activeItems = new Dictionary<Vector2Int, ItemController>();

    void Update()
    {
        // 毎フレーム、両方のプレイヤーについて取得判定を行う
        if (player1 != null) CheckForPickup(player1);
        if (player2 != null) CheckForPickup(player2);
    }

    /// <summary>
    /// 指定されたプレイヤーの位置にアイテムがあるか確認し、取得処理を行う
    /// </summary>
    private void CheckForPickup(PlayerController player)
    {
        // プレイヤーの現在座標を取得
        Vector2Int playerCoords = new Vector2Int(player.GetCurrentX(), player.GetCurrentY());

        // もし管理リストにプレイヤーの座標が存在したら（＝アイテムを踏んだら）
        if (activeItems.ContainsKey(playerCoords))
        {
            // その座標にあるアイテムの情報を取得
            ItemController itemToGet = activeItems[playerCoords];

            // アイテムの効果をプレイヤーに適用する（PlayerControllerに後で実装）
            switch (itemToGet.itemType)
            {
                case ItemController.ItemType.SpecialMachineGun:
                    //player.ApplyAttackSpeedBuff(itemToGet.effectDuration);
                    break;
                case ItemController.ItemType.SwiftBoots:
                    //player.ApplyCooldownReductionBuff(itemToGet.effectDuration);
                    break;
                case ItemController.ItemType.EnergyCider:
                    //player.ApplyStunImmunityBuff(itemToGet.effectDuration);
                    break;
            }

            // 取得されたので、管理リストから削除
            activeItems.Remove(playerCoords);
            // アイテムのゲームオブジェクトをシーンから破壊
            Destroy(itemToGet.gameObject);
        }
    }

    /// <summary>
    /// アイテムから呼び出され、自身を管理リストに登録してもらうための公開メソッド
    /// </summary>
    public void RegisterItem(Vector2Int coords, ItemController item)
    {
        if (!activeItems.ContainsKey(coords))
        {
            activeItems.Add(coords, item);
        }
    }

    /// <summary>
    /// アイテムから呼び出され、自身を管理リストから削除してもらうための公開メソッド
    /// </summary>
    public void UnregisterItem(Vector2Int coords)
    {
        if (activeItems.ContainsKey(coords))
        {
            activeItems.Remove(coords);
        }
    }
}