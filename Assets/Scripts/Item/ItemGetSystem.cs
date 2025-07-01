using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �V�[����̃A�C�e���ƃv���C���[���Ď����A�A�C�e���擾���Ǘ�����i�ߓ��B
/// </summary>
public class ItemGetSystem : MonoBehaviour
{
    [Header("�� �Ď��Ώۂ̃v���C���[")]
    public PlayerController player1;
    public PlayerController player2;

    // �V�[���ɑ��݂���S�A�C�e���̍��W�Ə����Ǘ����郊�X�g
    private Dictionary<Vector2Int, ItemController> activeItems = new Dictionary<Vector2Int, ItemController>();

    void Update()
    {
        // ���t���[���A�����̃v���C���[�ɂ��Ď擾������s��
        if (player1 != null) CheckForPickup(player1);
        if (player2 != null) CheckForPickup(player2);
    }

    /// <summary>
    /// �w�肳�ꂽ�v���C���[�̈ʒu�ɃA�C�e�������邩�m�F���A�擾�������s��
    /// </summary>
    private void CheckForPickup(PlayerController player)
    {
        // �v���C���[�̌��ݍ��W���擾
        Vector2Int playerCoords = new Vector2Int(player.GetCurrentX(), player.GetCurrentY());

        // �����Ǘ����X�g�Ƀv���C���[�̍��W�����݂�����i���A�C�e���𓥂񂾂�j
        if (activeItems.ContainsKey(playerCoords))
        {
            // ���̍��W�ɂ���A�C�e���̏����擾
            ItemController itemToGet = activeItems[playerCoords];

            // �A�C�e���̌��ʂ��v���C���[�ɓK�p����iPlayerController�Ɍ�Ŏ����j
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

            // �擾���ꂽ�̂ŁA�Ǘ����X�g����폜
            activeItems.Remove(playerCoords);
            // �A�C�e���̃Q�[���I�u�W�F�N�g���V�[������j��
            Destroy(itemToGet.gameObject);
        }
    }

    /// <summary>
    /// �A�C�e������Ăяo����A���g���Ǘ����X�g�ɓo�^���Ă��炤���߂̌��J���\�b�h
    /// </summary>
    public void RegisterItem(Vector2Int coords, ItemController item)
    {
        if (!activeItems.ContainsKey(coords))
        {
            activeItems.Add(coords, item);
        }
    }

    /// <summary>
    /// �A�C�e������Ăяo����A���g���Ǘ����X�g����폜���Ă��炤���߂̌��J���\�b�h
    /// </summary>
    public void UnregisterItem(Vector2Int coords)
    {
        if (activeItems.ContainsKey(coords))
        {
            activeItems.Remove(coords);
        }
    }
}