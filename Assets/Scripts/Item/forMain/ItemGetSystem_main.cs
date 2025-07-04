using System.Collections.Generic;
using UnityEngine;

// プレイヤーIDを定義
public enum PlayerID { P1, P2 }

public class ItemGetSystem_main : MonoBehaviour
{
    [Header("■ プレイヤー設定")]
    public PlayerID playerID;

    [Header("■ 監視対象")]
    public PlayerController myPlayer;
    public Curcor myCursor;

    // 全てのItemGetSystemを管理するリスト
    public static Dictionary<PlayerID, ItemGetSystem_main> Systems = new Dictionary<PlayerID, ItemGetSystem_main>();

    // このシステムが管理するアクティブなアイテムのリスト
    private Dictionary<Vector2Int, ItemController_main> activeItems = new Dictionary<Vector2Int, ItemController_main>();

    void Awake()
    {
        // 起動時に、自分自身をチームIDと紐づけてリストに登録
        Systems[playerID] = this;
    }

    void OnDestroy()
    {
        // 破壊される時にリストから自分を削除
        Systems.Remove(playerID);
    }

    void Update()
    {
        // 自分のチームのプレイヤーだけをチェック
        if (myPlayer != null) CheckForPickup(myPlayer);
    }

    private void CheckForPickup(PlayerController player)
    {
        Vector2Int playerCoords = new Vector2Int(player.GetCurrentX(), player.GetCurrentY());
        if (activeItems.ContainsKey(playerCoords))
        {
            ItemController_main itemToGet = activeItems[playerCoords];

            Debug.Log($"[{playerID}] がアイテム「{itemToGet.itemType}」を取得しました！");

            switch (itemToGet.itemType)
            {
                case ItemController_main.ItemType.SpecialMachineGun:
                    if (myCursor != null) myCursor.ApplyAttackSpeedBuff(itemToGet.effectDuration);
                    break;
                case ItemController_main.ItemType.SwiftBoots:
                    player.ApplyCooldownReductionBuff(itemToGet.effectDuration);
                    break;
                case ItemController_main.ItemType.EnergyCider:
                    player.ApplyStunImmunityBuff(itemToGet.effectDuration);
                    break;
            }
            activeItems.Remove(playerCoords);
            Destroy(itemToGet.gameObject);
        }
    }

    public void RegisterItem(Vector2Int coords, ItemController_main item)
    {
        if (!activeItems.ContainsKey(coords))
        {
            activeItems.Add(coords, item);
        }
    }

    public void UnregisterItem(Vector2Int coords)
    {
        if (activeItems.ContainsKey(coords))
        {
            activeItems.Remove(coords);
        }
    }
}