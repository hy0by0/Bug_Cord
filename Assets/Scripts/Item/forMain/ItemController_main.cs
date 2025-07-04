using UnityEngine;

public class ItemController_main : MonoBehaviour
{
    public enum ItemType { SpecialMachineGun, SwiftBoots, EnergyCider }
    [Header("ÉAÉCÉeÉÄê›íË")]
    public ItemType itemType;
    public float effectDuration = 8.0f;

    private Vector2Int myGridCoords;
    private ItemGetSystem_main itemGetSystem;
    private PlayerID myPlayerID;

    public void Initialize(PlayerID id, ItemSpawner_main spawner)
    {
        myPlayerID = id;

        if (ItemGetSystem_main.Systems.ContainsKey(myPlayerID))
        {
            itemGetSystem = ItemGetSystem_main.Systems[myPlayerID];
        }

        if (itemGetSystem != null && spawner != null)
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

            itemGetSystem.RegisterItem(myGridCoords, this);
        }
    }

    void OnDestroy()
    {
        if (itemGetSystem != null)
        {
            itemGetSystem.UnregisterItem(myGridCoords);
        }
    }
}