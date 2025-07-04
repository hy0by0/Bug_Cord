using System.Collections.Generic;
using UnityEngine;

// �v���C���[ID���`
public enum PlayerID { P1, P2 }

public class ItemGetSystem_main : MonoBehaviour
{
    [Header("�� �v���C���[�ݒ�")]
    public PlayerID playerID;

    [Header("�� �Ď��Ώ�")]
    public PlayerController myPlayer;
    public Curcor myCursor;

    // �S�Ă�ItemGetSystem���Ǘ����郊�X�g
    public static Dictionary<PlayerID, ItemGetSystem_main> Systems = new Dictionary<PlayerID, ItemGetSystem_main>();

    // ���̃V�X�e�����Ǘ�����A�N�e�B�u�ȃA�C�e���̃��X�g
    private Dictionary<Vector2Int, ItemController_main> activeItems = new Dictionary<Vector2Int, ItemController_main>();

    void Awake()
    {
        // �N�����ɁA�������g���`�[��ID�ƕR�Â��ă��X�g�ɓo�^
        Systems[playerID] = this;
    }

    void OnDestroy()
    {
        // �j�󂳂�鎞�Ƀ��X�g���玩�����폜
        Systems.Remove(playerID);
    }

    void Update()
    {
        // �����̃`�[���̃v���C���[�������`�F�b�N
        if (myPlayer != null) CheckForPickup(myPlayer);
    }

    private void CheckForPickup(PlayerController player)
    {
        Vector2Int playerCoords = new Vector2Int(player.GetCurrentX(), player.GetCurrentY());
        if (activeItems.ContainsKey(playerCoords))
        {
            ItemController_main itemToGet = activeItems[playerCoords];

            Debug.Log($"[{playerID}] ���A�C�e���u{itemToGet.itemType}�v���擾���܂����I");

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