using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTerrainGeneration : GenerationStage
{
    [SerializeField]
    private NoiseData noiseData;

    public override ChunkData ProcessChunk(ChunkData chunkData)
    {
        int chunkRes = worldData.ChunkResolution;

        // Создание карты шума в виде массива
        var noiseOffset = new Vector2(chunkData.ChunkPosition.X * worldData.ChunkSize,
            chunkData.ChunkPosition.Z * worldData.ChunkSize);

        // Создание матрицы шума, чтобы в дальнейшем назначить Terrain через TerrainData
        float[,] heights = NoiseMapUtils.GenerateNoiseMap(noiseData, worldData.Seed,
            chunkRes, chunkRes, noiseOffset, worldData.WorldScale);

        // Применение карты высот и настроек к TerrainData
        chunkData.TerrainData.size = new Vector3(worldData.ChunkSize,
            worldData.ChunkHeight / worldData.WorldScale, worldData.ChunkSize);
        
        chunkData.TerrainData.heightmapResolution = chunkRes;
        chunkData.TerrainData.SetHeights(0, 0, heights);
        
        return chunkData;
    }
}
