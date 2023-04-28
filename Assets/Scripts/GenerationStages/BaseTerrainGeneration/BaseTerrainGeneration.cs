using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTerrainGeneration : MonoBehaviour, IGenerationStage
{
    [SerializeField]
    private NoiseData noiseData;
    public void Initialize() {
        
    }
    public ChunkData ProcessChunk(WorldData worldData, ChunkData chunkData)
    {
        int heightsSize = worldData.HeightsSize;

        // Создание карты шума в виде массива
        var noiseOffset = new Vector2(chunkData.ChunkPosition.X * worldData.ChunkSize,
            chunkData.ChunkPosition.Z * worldData.ChunkSize);

        // Создание матрицы шума, чтобы в дальнейшем назначить Terrain через TerrainData
        float[,] heights = NoiseMapUtils.GenerateNoiseMap(noiseData, worldData.Seed,
            heightsSize, heightsSize, noiseOffset);

        // Применение карты высот и настроек к TerrainData
        chunkData.TerrainData.size = new Vector3(worldData.ChunkSize,
            worldData.ChunkHeight, worldData.ChunkSize);
        
        chunkData.TerrainData.heightmapResolution = heightsSize;
        chunkData.TerrainData.SetHeights(0, 0, heights);
        
        return chunkData;
    }
}
