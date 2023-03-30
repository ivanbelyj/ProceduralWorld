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
        GameObject terrainGO = Terrain.CreateTerrainGameObject(chunkData.TerrainData);
        Debug.Log("Создание чанка на позиции "
            + chunkData.ChunkPosition.X + " " + chunkData.ChunkPosition.Y);
        terrainGO.transform.position = new Vector3(worldData.ChunkSize * chunkData.ChunkPosition.X, 0,
            worldData.ChunkSize * chunkData.ChunkPosition.Y);
        return terrainGO;
    }

    public void Initialize(WorldData worldData) {
        this.worldData = worldData;
    }
}
