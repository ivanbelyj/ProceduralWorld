using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонен, позволяющий создавать чанки на сцене
/// </summary>
public class WorldBuilder : MonoBehaviour
{
    private WorldData worldData;

    [SerializeField]
    private GameObject terrainPrefab;

    /// <summary>
    /// На основе ChunkData создает игровой объект чанка на сцене и возвращает его
    /// </summary>
    public GameObject CreateChunkGO(ChunkData chunkData) {
        GameObject terrainGO = Instantiate(terrainPrefab);
        Debug.Log("Создание чанка на позиции " + chunkData.ChunkPosition);
        terrainGO.transform.position = new Vector3(worldData.ChunkWidth * chunkData.ChunkPosition.X, 0,
            worldData.ChunkLength * chunkData.ChunkPosition.Y);
        
        Terrain terrain = terrainGO.GetComponent<Terrain>();
        if (terrain == null)
            Debug.LogError("Terrain prefab has no Terrain component attached");
        return null;
    }

    public void Initialize(WorldData worldData) {
        this.worldData = worldData;
    }
}
