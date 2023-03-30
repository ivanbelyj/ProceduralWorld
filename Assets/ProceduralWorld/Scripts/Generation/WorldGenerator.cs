using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Предоставляет возможности генерации чанков пользователю
/// </summary>
public class WorldGenerator
{
    private WorldData worldData;
    private List<IGenerationStage> generationStages;
    // private TerrainData initialTerrainData;

    /// <summary>
    /// Устанавливает исходные данные о мире перед тем, как генерировать чанки
    /// </summary>
    public void Initialize(WorldData wordData) {
        this.worldData = wordData;
        // this.initialTerrainData = initialTerrainData;

        Random.InitState(wordData.Seed);

        generationStages = new List<IGenerationStage>();
        AddGenerationStages();
    }

    /// <summary>
    /// Добавление объектов, осуществляющих генерацию (порядок имеет значение)
    /// </summary>
    private void AddGenerationStages() {
        generationStages.Add(new BaseTerrainGeneration());
    }

    /// <summary>
    /// Генерирует данные чанка, расположенного по заданной позиции
    /// </summary>
    public ChunkData GenerateChunk(ChunkPosition chunkPos) {
        if (generationStages.Count == 0)
            throw new System.InvalidOperationException(
                "Generation stages must be set before chunk generation");
        
        ChunkData initialChunkData = new ChunkData() {
            ChunkPosition = chunkPos,
            TerrainData = CreateInitialTerrainData()
        };

        ChunkData lastProcessed = generationStages[0].ProcessChunk(worldData,
            initialChunkData);
        foreach (var stage in generationStages) {
            lastProcessed = stage.ProcessChunk(worldData, lastProcessed);
        }
        return lastProcessed;
    }

    /// <summary>
    /// Создание изначальных данных ландшафта, которые далее будут переданы на обработку
    /// </summary>
    private TerrainData CreateInitialTerrainData() {
        return new TerrainData();
    }
}
