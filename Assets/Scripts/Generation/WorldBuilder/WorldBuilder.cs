using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Этап генерации, создающий чанки на сцене
/// </summary>
public class WorldBuilder : GenerationStage
{
    private Dictionary<ChunkPosition, Terrain> createdTerrains;
    private GameObject chunksParent;
    

    public override void Initialize(WorldGenerationData worldData) {
        base.Initialize(worldData);

        this.worldData = worldData;
        createdTerrains = new Dictionary<ChunkPosition, Terrain>();

        chunksParent = new GameObject("Chunks");
    }

    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        chunkData = base.ProcessChunk(chunkData);
        CreateChunkGO(chunkData);
        return chunkData;
    }

    /// <summary>
    /// На основе ChunkData создает игровой объект чанка на сцене и возвращает его
    /// </summary>
    public GameObject CreateChunkGO(ChunkData chunkData) {
        GameObject terrainGO = Terrain.CreateTerrainGameObject(chunkData.TerrainData);
        terrainGO.transform.SetParent(chunksParent.transform);
        terrainGO.transform.position = new Vector3(worldData.ChunkSize * chunkData.ChunkPosition.X, 0,
            worldData.ChunkSize * chunkData.ChunkPosition.Z);

        var terrain = chunkData.Terrain = terrainGO.GetComponent<Terrain>();
        createdTerrains[chunkData.ChunkPosition] = terrain;
        
        ApplyTerrainSettings(terrain);
        UpdateNeighbors(chunkData.ChunkPosition);

        return terrainGO;
    }

    private void ApplyTerrainSettings(Terrain terrain) {
        terrain.treeBillboardDistance = 1000;
        terrain.detailObjectDistance = 250;
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
                UpdateNeighbors(neighbor, setForNeighbors: false);
            }
        }
    }

    private Terrain GetTerrainByPos(ChunkPosition cPos)
        => createdTerrains.ContainsKey(cPos) ? createdTerrains[cPos] : null;
}
