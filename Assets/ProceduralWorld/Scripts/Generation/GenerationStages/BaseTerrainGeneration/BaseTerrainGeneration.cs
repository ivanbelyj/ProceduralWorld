using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTerrainGeneration : IGenerationStage
{
    public ChunkData ProcessChunk(WorldData worldData, ChunkData chunkData)
    {
        // Размерность карты высот и зависимых от нее параметров (разрешение).
        // Превосходит размер чанка на единицу, т.к. карта высот накладывается
        // не на "квадраты", из которых состоят чанки, а на вершины сетки
        int mapSize = worldData.ChunkSize + 1;

        // Создание карты шума в виде массива
        var noise = worldData.NoiseData;
        var noiseOffset = new Vector2(chunkData.ChunkPosition.X * worldData.ChunkSize,
            chunkData.ChunkPosition.Y * worldData.ChunkSize);

        float[] noiseMap = NoiseMapUtils.GenerateNoiseMap(mapSize,
            mapSize, worldData.Seed, worldData.Scale,
            noise.Octaves, noise.Persistence, noise.Lacunarity,
            noiseOffset);

        // Color[] colorMap = NoiseMapToTextureUtils.NoiseMapToColorMap(noiseMap);
        // Texture2D noiseTexture = NoiseMapToTextureUtils.ColorMapToTexture(
        //     worldData.ChunkSize, worldData.ChunkHeight, colorMap);

        // Создание матрицы шума, чтобы в дальнейшем назначить Terrain через TerrainData
        float[,] heights = new float[mapSize, mapSize];
        // for (int i = 0; i < noiseMap.Length; i++) {
        //     heights[i / mapSize, i % mapSize] = noiseMap[i];
        // }
        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                heights[y, x] = noiseMap[y * mapSize + x];
            }
        }

        // Применение карты высот и настроек к TerrainData
        chunkData.TerrainData.size = new Vector3(worldData.ChunkSize,
            worldData.ChunkHeight, worldData.ChunkSize);
        
        chunkData.TerrainData.heightmapResolution = mapSize;
        chunkData.TerrainData.SetHeights(0, 0, heights);
        
        return chunkData;
    }
}
