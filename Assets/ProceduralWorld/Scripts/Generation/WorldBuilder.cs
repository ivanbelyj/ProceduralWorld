using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Компонен, позволяющий создавать чанки на сцене
/// </summary>
public class WorldBuilder : MonoBehaviour
{
    private WorldData worldData;

    private Dictionary<ChunkPosition, Terrain> createdTerrains;

    [SerializeField]
    private GameObject terrainPrefab;

    public void Initialize(WorldData worldData) {
        this.worldData = worldData;
        createdTerrains = new Dictionary<ChunkPosition, Terrain>();
    }

    /// <summary>
    /// На основе ChunkData создает игровой объект чанка на сцене и возвращает его
    /// </summary>
    public GameObject CreateChunkGO(ChunkData chunkData) {
        GameObject terrainGO = Terrain.CreateTerrainGameObject(chunkData.TerrainData);
        terrainGO.transform.position = new Vector3(worldData.ChunkSize * chunkData.ChunkPosition.X, 0,
            worldData.ChunkSize * chunkData.ChunkPosition.Y);

        createdTerrains[chunkData.ChunkPosition] = terrainGO.GetComponent<Terrain>();
        Debug.Log("Создание чанка на позиции "
            + chunkData.ChunkPosition.X + " " + chunkData.ChunkPosition.Y);
        
        // UpdateNeighbors(chunkData.ChunkPosition);

        return terrainGO;
    }

    /// <summary>
    /// Обновляет существующие соседние Terrain для созданного Terrain'а по заданной позиции.
    /// setForNeighbors: true, если требуется обновить соседей не только самого элемента,
    /// но и его соседей
    /// </summary>
    private void UpdateNeighbors(ChunkPosition cPos, bool setForNeighbors = true) {
        if (setForNeighbors)
            Debug.Log("Обновление соседей для " + cPos);
        // Установка соседей
        Terrain terrain = createdTerrains[cPos];
        
        Terrain top = GetTerrainByPos(cPos.Top);
        Terrain right = GetTerrainByPos(cPos.Right);
        Terrain bottom = GetTerrainByPos(cPos.Bottom);
        Terrain left = GetTerrainByPos(cPos.Left);

        terrain.SetNeighbors(left, top, right, bottom);

        if (setForNeighbors) {
            foreach (var neighbor in new [] { cPos.Top, cPos.Right, cPos.Bottom, cPos.Left }
                .Where(pos => createdTerrains.ContainsKey(pos)) ) {
                Debug.Log("\tОбновление соседей для соседа. " + neighbor);
                UpdateNeighbors(neighbor, setForNeighbors: false);
            }
        }
    }

    private Terrain GetTerrainByPos(ChunkPosition cPos)
        => createdTerrains.ContainsKey(cPos) ? createdTerrains[cPos] : null;
}
