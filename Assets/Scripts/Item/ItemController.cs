using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�A�C�e�������u�����̏��v���Ǘ����AItemGetSystem�Ɏ��g�̑��݂�ʒm����B
/// </summary>
public class ItemController : MonoBehaviour
{
    public enum ItemType { SpecialMachineGun, SwiftBoots, EnergyCider }
    [Header("�A�C�e���ݒ�")]
    public ItemType itemType;
    public float effectDuration = 8.0f;

    private Vector2Int myGridCoords;
    private ItemGetSystem itemGetSystem;

    void Start()
    {
        // �i�ߓ��ł���ItemGetSystem���V�[�����猩����
        itemGetSystem = FindObjectOfType<ItemGetSystem>();
        if (itemGetSystem == null)
        {
            Destroy(gameObject); // �i�ߓ������Ȃ��Ȃ����
            return;
        }

        // �����̃O���b�h���W���v�Z���ē��肷��
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

            // �i�ߓ��Ɏ����̑��݂�o�^
            itemGetSystem.RegisterItem(myGridCoords, this);
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g���j�󂳂�鎞�Ɏ����ŌĂ΂��
    /// </summary>
    void OnDestroy()
    {
        // �i�ߓ����܂����݂���Ȃ�A�Ǘ����X�g���玩���̏����폜���Ă��炤
        if (itemGetSystem != null)
        {
            itemGetSystem.UnregisterItem(myGridCoords);
        }
    }
}