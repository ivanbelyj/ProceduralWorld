using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Предоставляет возможности генерации чанков пользователю
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    private WorldGenerationData worldData;
    private List<IGenerationStage> generationStages;

    [SerializeField]
    private BaseTerrainGeneration baseTerrainGeneration;

    [SerializeField]
    private BiomesGeneration biomesGeneration;

    [SerializeField]
    private FoliageGeneration foliageGeneration;

    /// <summary>
    /// Устанавливает исходные данные о мире перед тем, как генерировать чанки
    /// </summary>
    public void Initialize(WorldGenerationData wordData) {
        this.worldData = wordData;
        // this.initialTerrainData = initialTerrainData;

        Random.InitState(wordData.Seed);

        generationStages = new List<IGenerationStage>();
        AddGenerationStages();
    }

    /// <summary>
    /// Добавление объектов, осуществляющих генерацию
    /// </summary>
    private void AddGenerationStages() {
        generationStages.Add(baseTerrainGeneration);
        generationStages.Add(biomesGeneration);
        generationStages.Add(foliageGeneration);

        foreach(var stage in generationStages) {
            stage.Initialize(worldData);
        }
    }

    /// <summary>
    /// Генерирует данные чанка, расположенного по заданной позиции
    /// </summary>
    public ChunkData GenerateChunk(ChunkPosition chunkPos) {
        if (generationStages.Count == 0)
            throw new System.InvalidOperationException(
                "Generation stages must be set before chunk generation");
        
        var terrainData = CreateInitialTerrainData();
        ChunkData initialChunkData = new ChunkData() {
            ChunkPosition = chunkPos,
            TerrainData = terrainData
        };

        ChunkData lastProcessed = generationStages[0].ProcessChunk(initialChunkData);
        foreach (var stage in generationStages) {
            lastProcessed = stage.ProcessChunk(lastProcessed);
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
