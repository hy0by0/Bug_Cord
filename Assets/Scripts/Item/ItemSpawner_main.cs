using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner_main : MonoBehaviour
{
    [Header("�� �v���C���[�ݒ�")]
    public PlayerID playerID;

    [Header("�� �֘A�I�u�W�F�N�g")]
    public PlayerController myPlayer;

    [Header("�� �z�u�ݒ�")]
    public List<Transform> positionMarkers;
    public List<GameObject> itemPrefabs;

    [Header("�� ���Ԑݒ�")]
    public float spawnInterval = 15.0f;
    public float itemLifetime = 10.0f;
    public int maxItems = 3;

    [Header("�� ���ߖ@�i�p�[�X�j�ݒ�")]
    public float frontRowScale = 1.0f;
    public float backRowScale = 0.7f;

    private Dictionary<Vector2Int, GameObject> spawnedItems = new Dictionary<Vector2Int, GameObject>();

    // �������y�ǉ��z����Start()�֐��������Ă��܂��� ������
    void Start()
    {
        // �v���C���[���C���X�y�N�^�[�Őݒ肳��Ă��邩�`�F�b�N
        if (myPlayer == null)
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