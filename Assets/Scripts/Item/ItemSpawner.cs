using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�⑼�̃A�C�e���̈ʒu���l�����A�󂢂Ă���}�X�ɂ̂݃A�C�e���𐶐�����B
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [Header("�� �֘A�I�u�W�F�N�g")]
    [Tooltip("�v���C���[1")]
    public PlayerController playerController1;
    [Tooltip("�v���C���[2")]
    public PlayerController playerController2;

    [Header("�� �z�u�ݒ�")]
    [Tooltip("�A�C�e����z�u����ꏊ�̖ڈ�ƂȂ�Transform��9�ݒ肵�܂�")]
    public List<Transform> positionMarkers;
    [Tooltip("�o��������A�C�e���̃v���n�u�𕡐��ݒ�ł��܂�")]
    public List<GameObject> itemPrefabs;

    [Header("�� ���Ԑݒ�")]
    [Tooltip("�A�C�e�����V�����z�u�����܂ł̊Ԋu�i�b�j")]
    public float spawnInterval = 15.0f;
    [Tooltip("�o�������A�C�e�����V�[�����玩���ŏ�����܂ł̎��ԁi�b�j")]
    public float itemLifetime = 10.0f;
    [Tooltip("�����ɑ��݂ł���A�C�e�����̏��")]
    public int maxItems = 3;

    [Header("�� ���ߖ@�i�p�[�X�j�ݒ�")]
    [Tooltip("��Ԏ�O�̃}�X�ł̃A�C�e���̑傫���̔{�� (��: 1.0)")]
    public float frontRowScale = 1.0f;
    [Tooltip("��ԉ��̃}�X�ł̃A�C�e���̑傫���̔{�� (��: 0.7)")]
    public float backRowScale = 0.7f;

    // �������ꂽ�A�C�e���Ƃ��̃O���b�h���W���Z�b�g�ŋL�����鎫��
    private Dictionary<Vector2Int, GameObject> spawnedItems = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // �v���C���[���C���X�y�N�^�[�Őݒ肳��Ă��邩�`�F�b�N
        if (playerController1 == null || playerController2 == null)
        {
            Debug.LogError("ItemSpawner�Ƀv���C���[���ݒ肳��Ă��܂���I�C���X�y�N�^�[���m�F���Ă��������B", this);
            enabled = false;
            return;
        }

        // �A�C�e�������̌J��Ԃ��������J�n
        StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator SpawnItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // ���݂̃A�C�e����������ɒB���Ă�����A�������Ȃ�
            if (spawnedItems.Count >= maxItems)
            {
                continue;
            }

            // --- �z�u�ς݂̃}�X�i�v���C���[�Ɗ����A�C�e���j�����X�g�A�b�v ---
            HashSet<Vector2Int> occupiedCoordinates = new HashSet<Vector2Int>();

            occupiedCoordinates.Add(new Vector2Int(playerController1.GetCurrentX(), playerController1.GetCurrentY()));
            occupiedCoordinates.Add(new Vector2Int(playerController2.GetCurrentX(), playerController2.GetCurrentY()));

            foreach (Vector2Int itemCoords in spawnedItems.Keys)
            {
                occupiedCoordinates.Add(itemCoords);
            }

            // --- �z�u�\�ȁu�󂫃}�X�v��T�� ---
            List<int> availableIndexes = new List<int>();
            for (int i = 0; i < positionMarkers.Count; i++)
            {
                Vector2Int currentCoords = new Vector2Int(i % 3, i / 3);
                if (!occupiedCoordinates.Contains(currentCoords))
                {
                    availableIndexes.Add(i);
                }
            }

            // --- �����󂫃}�X��1�ł�����΁A�����ɐ��� ---
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

        // ���Ԍo�ߌ�A���̃A�C�e�����܂����݁i�N�ɂ��擾����Ă��Ȃ��j���Ă���Δj��
        if (spawnedItems.ContainsKey(coords) && spawnedItems[coords] == item)
        {
            Destroy(item);
            spawnedItems.Remove(coords);
        }
    }
}